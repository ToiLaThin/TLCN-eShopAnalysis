import pandas as pd
from pyspark.ml.recommendation import ALS, ALSModel
from pyspark import SparkContext
from pyspark.sql import SparkSession
from utils.share_util import get_root_path
import findspark
import os
from flask import request

findspark.init('D:\Hadoop_Ecosystem\spark-3.5.0-bin-hadoop3')
sc = SparkContext(appName="ALS").getOrCreate()
sc.setLogLevel("ERROR")
ss = SparkSession(sc)

def train_model(file_path):
    df = pd.read_csv(get_root_path() + '/resources/product_user_interactions.csv')
    df['user_id'] = df['user_key'].astype('category').cat.codes
    df['product_id'] = df['product_key'].astype('category').cat.codes
    als = ALS(maxIter=5, regParam=0.01, userCol='user_id', itemCol='product_id', ratingCol='rating', coldStartStrategy='drop')
    als_trained = als.fit(ss.createDataFrame(df))
    predictions = als_trained.transform(ss.createDataFrame(df))
    predictions.show()
    als_trained.write().overwrite().save(file_path)
    return als_trained

def recommendation(file_path, user_key='0197637f-32b5-45e7-afb1-afd6408ab4e2'):
    # enable this when fixxed hadoop & spark...
    # if not os.path.exists(file_path):
    #     print('no model file found. will train the model first.')
    #     train_model(file_path)
    # als_trained = ALSModel.load(file_path)
    als_trained = train_model(file_path)

    df = pd.read_csv(get_root_path() + '/resources/product_user_interactions.csv')
    df['user_id'] = df['user_key'].astype('category').cat.codes
    df['product_id'] = df['product_key'].astype('category').cat.codes
    if not os.path.exists(get_root_path() + '/resources/recommendation_product_mappings.csv'):
        print('empty product mapping file. creating new one.')
        recommendation_product_mappings = df[['product_key', 'product_id']].drop_duplicates().sort_values(by='product_id')
        recommendation_product_mappings.reset_index(drop=True, inplace=True)
        recommendation_product_mappings.to_csv(get_root_path() + '/resources/recommendation_product_mappings.csv', index=False)
    recommendation_product_mappings = pd.read_csv(get_root_path() + '/resources/recommendation_product_mappings.csv')
    
    if not os.path.exists(get_root_path() + '/resources/recommendation_user_mappings.csv'):
        print('empty user mapping file. creating new one.')
        recommendation_user_mappings = df[['user_key', 'user_id']].drop_duplicates().sort_values(by='user_id')
        recommendation_user_mappings.reset_index(drop=True, inplace=True)
        recommendation_user_mappings.to_csv(get_root_path() + '/resources/recommendation_user_mappings.csv', index=False)
    recommendation_user_mappings = pd.read_csv(get_root_path() + '/resources/recommendation_user_mappings.csv')

    user_id = recommendation_user_mappings[recommendation_user_mappings['user_key'] == user_key].user_id.values[0]
    recommendations = als_trained.recommendForAllUsers(10).filter(f'user_id == {user_id}').collect()
    recommended_products = recommendations[0].recommendations
    result_json_list = []
    for p in recommended_products:
        result_json = {}
        product_key = recommendation_product_mappings[recommendation_product_mappings['product_id'] == p['product_id']].product_key.values[0]
        result_json['product_key'] = product_key
        result_json['rating'] = p['rating']
        result_json_list.append(result_json)
    return result_json_list

def recommend():
    user_id = request.args.get('user_id')
    return recommendation(get_root_path() + '/resources/als_model', user_key=user_id)