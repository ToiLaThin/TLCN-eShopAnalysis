import pandas as pd
from pyspark.ml.recommendation import ALS, ALSModel
from pyspark import SparkContext
from pyspark.sql import SparkSession
from utils.share_util import get_root_path, init_spark_session
import findspark
import os
from flask import request


def recommend_collaborate_based():
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
    
    def generate_recommendations(file_path, user_key='0197637f-32b5-45e7-afb1-afd6408ab4e2'):
        # enable this when fixxed hadoop & spark...
        # if not os.path.exists(file_path):
        #     print('no model file found. will train the model first.')
        #     train_model(file_path)
        # als_trained = ALSModel.load(file_path)        
        als_trained = train_model(file_path)
        
        # TODO: can change to load data from hdfs
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
        ss.stop() # should stop here, because we don't need to use SparkSession anymore, this is the ss we init in the parent function
        return result_json_list
    
    ss = init_spark_session() #should be before create ALS, because ALS need to use SparkSession, we should not stop before the last time we ref als_trained
    user_id = request.args.get('user_id')
    # hdfs path cannot loaded too, still have error cannot load native hadoop library
    return generate_recommendations(get_root_path() + '/resources/als_model', user_key=user_id) 

from hdfs import InsecureClient
import numpy as np
import pandas as pd
from sklearn.metrics.pairwise import linear_kernel
from sklearn.feature_extraction.text import TfidfVectorizer
from pandas import isnull, notnull

def recommend_content_based() -> list[str]:
    def get_product_features_data(by_key=False):
        namenode_url = 'http://localhost:9870'
        hdfs_client = InsecureClient(namenode_url, user='root')
        upload_folder = '/user/root/upload'
        product_features_file = '/user/root/upload/product_features.csv'
        product_ga4_file = '/user/root/upload/product_ga4_data.csv'
        print(hdfs_client.list(upload_folder))
        with hdfs_client.read(product_features_file) as reader:
            product_features_df = pd.read_csv(reader)
        with hdfs_client.read(product_ga4_file) as reader:
            product_ga4_df = pd.read_csv(reader)
        product_df = pd.merge(product_features_df, product_ga4_df, left_on='BusinessKey', right_on='p_business_key', how='inner')
        product_df = product_df[['p_name', 'ProductBrandName', 'CatalogName', 'Price', 'p_subcatalog', 'p_business_key', 'views', 'add_to_carts', 'purchases']]
        product_df.rename(
            columns={
                'ProductBrandName': 'p_brand', 
                'CatalogName': 'p_catalog', 
                'Price': 'p_price'
            }, 
            inplace=True
        )
        product_df['views'] = pd.cut(product_df['views'], bins=3, labels=['views_low', 'views_medium', 'views_high'])
        # bin the price in the context of the product only in the same subcatalog
        # product_df['p_price'] = pd.cut(product_df['p_price'], bins=3, labels=['price_low', 'price_medium', 'price_high'])
        product_df['p_price'] = product_df.groupby('p_subcatalog')['p_price'].transform(lambda x: pd.cut(x, bins=3, labels=['price_low', 'price_medium', 'price_high']))
        product_df['features'] =product_df['p_subcatalog'] + ' ' + \
                            product_df['views'].astype(str) + ' ' + \
                            product_df['p_brand'].astype(str) + ' ' + \
                            product_df['p_catalog'].astype(str) + ' ' + \
                            product_df['p_price'].astype(str)

        if by_key == False:
            product_df = product_df[['features', 'p_name']]
        else:
            product_df = product_df[['features', 'p_business_key']]
        return product_df

    def get_tfidf_matrix(df, column_name):
        """
            Dùng hàm "TfidfVectorizer" để chuẩn hóa "genres" với:
            + analyzer='word': chọn đơn vị trích xuất là word
            + ngram_range=(1, 1): mỗi lần trích xuất 1 word
            + min_df=0: tỉ lệ word không đọc được là 0
            Lúc này ma trận trả về với số dòng tương ứng với số lượng film và số cột tương ứng với số từ được tách ra từ "genres"
        """
        tf = TfidfVectorizer(analyzer='word', ngram_range=(1, 1), min_df=0.0)
        new_tfidf_matrix = tf.fit_transform(df[column_name])
        return new_tfidf_matrix
    
    def get_cosine_sim(matrix) -> np.ndarray:
        """
                Dùng hàm "linear_kernel" để tạo thành ma trận hình vuông với số hàng và số cột là số lượng film
                để tính toán điểm tương đồng giữa từng bộ phim với nhau
        """
        new_cosine_sim = linear_kernel(matrix, matrix)
        return new_cosine_sim
    
    def generate_recommendations_from_pname(product_name, top_x, df, cosine_sim):
        """
            Xây dựng hàm trả về danh sách top film tương đồng theo tên film truyền vào:
            + Tham số truyền vào gồm "title" là tên film và "topX" là top film tương đồng cần lấy
            + Tạo ra list "sim_score" là danh sách điểm tương đồng với film truyền vào
            + Sắp xếp điểm tương đồng từ cao đến thấp
            + Trả về top danh sách tương đồng cao nhất theo giá trị "topX" truyền vào
            Recommended products are numpy array, convert to list to return
        """
        names = df['p_name']
        indices = pd.Series(df.index, index=df['p_name'])
        idx = indices[product_name] 
        sim_scores = list(enumerate(cosine_sim[idx]))
        sim_scores = sorted(sim_scores, key=lambda x: x[1], reverse=True)
        sim_scores = sim_scores[1:top_x + 1]
        movie_indices = [i[0] for i in sim_scores]
        print(names.iloc[movie_indices].values)
        return sim_scores, names.iloc[movie_indices].values
    
    def generate_recommendations_from_pkey(product_key, top_x, df, cosine_sim):
        """
            Xây dựng hàm trả về danh sách top film tương đồng theo tên film truyền vào:
            + Tham số truyền vào gồm "title" là tên film và "topX" là top film tương đồng cần lấy
            + Tạo ra list "sim_score" là danh sách điểm tương đồng với film truyền vào
            + Sắp xếp điểm tương đồng từ cao đến thấp
            + Trả về top danh sách tương đồng cao nhất theo giá trị "topX" truyền vào
            Recommended products are numpy array, convert to list to return
        """
        all_p_keys = df['p_business_key']
        indices = pd.Series(df.index, index=df['p_business_key'])
        idx = indices[product_key] 
        sim_scores = list(enumerate(cosine_sim[idx]))
        sim_scores = sorted(sim_scores, key=lambda x: x[1], reverse=True)
        sim_scores = sim_scores[1:top_x + 1]
        movie_indices = [i[0] for i in sim_scores]
        print(all_p_keys.iloc[movie_indices].values)
        return sim_scores, all_p_keys.iloc[movie_indices].values
    
    product_key = request.args.get('product_key')
    hive_content_recommender_db_name = 'recommender_db'
    hive_content_recommender_table_name = 'product_cosine_sim'
    ss = init_spark_session()    
    product_df = get_product_features_data(by_key=True) # if gen recommendations by product key, set by_key=True

    is_db_exists = ss.sql(f"SHOW DATABASES LIKE '{hive_content_recommender_db_name}'").count() > 0
    if is_db_exists == False:
        print(f"Database {hive_content_recommender_db_name} does not exist. Creating...")
        ss.sql(f"CREATE DATABASE {hive_content_recommender_db_name}")
        ss.sql(f"USE {hive_content_recommender_db_name}")
    is_table_exists = ss.sql(f"SHOW TABLES IN {hive_content_recommender_db_name} LIKE '{hive_content_recommender_table_name}'").count() > 0
    if is_table_exists == False:
        print(f"Table {hive_content_recommender_table_name} does not exist. Creating...")
        ss.sql("""
            CREATE TABLE IF NOT EXISTS recommender_db.product_cosine_sim (
            p_name1 STRING, 
            p_name2 STRING, 
            cosine_sim FLOAT) 
            USING hive"""
        )

    hive_table_not_have_data = ss.sql(f"SELECT * FROM {hive_content_recommender_db_name}.{hive_content_recommender_table_name}").count() == 0
    hive_table_not_have_data = True # TODO after fixing wrong data returned, remove this line
    num_related_products = 3
    if hive_table_not_have_data == True:
        print('No data in product_cosine_sim table. Generating...')
        tfidf_matrix = get_tfidf_matrix(product_df, 'features')
        cosine_sim_ndarray = get_cosine_sim(tfidf_matrix)

        # transform ndarray to df and save to hive table, even if we use p_business_key, we can save df with p_name, because the order is the same, not modified
        temp_product_df = get_product_features_data(by_key=False)
        cosine_sim_df = pd.DataFrame(cosine_sim_ndarray, columns=temp_product_df['p_name'], index=temp_product_df['p_name'])
        cosine_sim_df = pd.melt(cosine_sim_df.reset_index(), id_vars='p_name', var_name='p_name2', value_name='cosine_sim')
        cosine_sim_df.columns = ['p_name1', 'p_name2', 'cosine_sim']

        # convert to spark df and save to hive table
        # ss.createDataFrame(cosine_sim_df).write.mode("overwrite").insertInto(f"{hive_content_recommender_db_name}.{hive_content_recommender_table_name}")
        # do not use insertInto, because it will append data, use saveAsTable instead to overwrite
        ss.createDataFrame(cosine_sim_df).write.mode("overwrite").saveAsTable(f"{hive_content_recommender_db_name}.{hive_content_recommender_table_name}")
        print(ss.sql(f'SELECT * FROM {hive_content_recommender_db_name}.{hive_content_recommender_table_name}').show())
        # result, recommended_product_names = generate_recommendations_from_pname('Bellany Coconut Paletas (110ml)', num_related_products + 1, product_df, cosine_sim_ndarray)
        result, recommended_product_keys = generate_recommendations_from_pkey(product_key, num_related_products + 1, product_df, cosine_sim_ndarray)
    else:
        # TODO wrong data returned, need to check why
        product_cosine_df = ss.sql("SELECT * FROM recommender_db.product_cosine_sim").toPandas()
        cosine_sim_df = product_cosine_df.pivot(index='p_name1', columns='p_name2', values='cosine_sim')
        cosine_sim_ndarray = cosine_sim_df.to_numpy()
        # result, recommended_product_names = generate_recommendations_from_pname('Bellany Coconut Paletas (110ml)', num_related_products + 1, product_df, cosine_sim_ndarray)
        result, recommended_product_keys = generate_recommendations_from_pkey(product_key, num_related_products + 1, product_df, cosine_sim_ndarray)
    ss.stop()
    buff = recommended_product_keys.tolist()
    print(buff)
    return buff
