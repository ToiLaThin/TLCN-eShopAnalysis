import pandas as pd
import logging
from shared.get_connections import get_cursor_mssql

from dotenv import load_dotenv
import os
load_dotenv('config.env')

class DimLookUp:
    """sumary_line
    create class to store the lookup table, so that we don't need to query the db everytime
    since pd.DataFrame is a immutable object, we can't pass by ref
    https://stackoverflow.com/a/986145
    Keyword arguments:
    argument -- description
    Return: return_description
    """    
    def __init__(self):
        self.lookup_brands_df: pd.DataFrame = pd.DataFrame()
        self.lookup_subcatalogs_df: pd.DataFrame = pd.DataFrame()

    def set_lookup_brands_df(self, brands_df: pd.DataFrame):
        self.lookup_brands_df = brands_df
    
    def set_lookup_subcatalogs_df(self, subcatalogs_df: pd.DataFrame):
        self.lookup_subcatalogs_df = subcatalogs_df

def map_product_brand_to_fkey(brand_name: str, dim_lookup: DimLookUp) -> int:
    """
    Get product brand fkey. 
    First we check if there already have data in the lookup df (from dim_lookup obj). If is, fetch data
    Then we get the brand key from the lookup df
    """
    if dim_lookup.lookup_brands_df.empty:
        try:
            mssql_cursor = get_cursor_mssql()
            brand_table_name = os.environ.get('MSSQL_DIM_BRAND_TABLE_NAME')
            brands_df = pd.read_sql_query(f'SELECT * FROM {brand_table_name}', mssql_cursor.connection)
            dim_lookup.set_lookup_brands_df(brands_df)
            # print(dim_lookup.lookup_brands_df)
            mssql_cursor.close()
            logging.log(logging.INFO, 'Connection to MSSQL closed.')
        except Exception as e:
            logging.log(logging.ERROR, e)
            raise Exception(e)
    brand_key = dim_lookup.lookup_brands_df[dim_lookup.lookup_brands_df['BrandName'] == brand_name].get('BrandId').values[0]
    return brand_key

def map_product_subcatalog_to_fkey(subcatalog_name: str, dim_lookup: DimLookUp) -> int:
    """
    Get product subcatalog fkey. 
    First we check if there already have data in the lookup df (from dim_lookup obj). If is, fetch data
    Then we get the subcatalog key from the lookup df
    """
    if dim_lookup.lookup_subcatalogs_df.empty:
        try:
            mssql_cursor = get_cursor_mssql()
            subcatalog_table_name = os.environ.get('MSSQL_DIM_SUBCATALOG_TABLE_NAME')
            subcatalogs_df = pd.read_sql_query(f'SELECT * FROM {subcatalog_table_name}', mssql_cursor.connection)
            dim_lookup.set_lookup_subcatalogs_df(subcatalogs_df)
            # print(dim_lookup.lookup_subcatalogs_df)
            mssql_cursor.close()
            logging.log(logging.INFO, 'Connection to MSSQL closed.')
        except Exception as e:
            logging.log(logging.ERROR, e)
            raise Exception(e)
    subcatalog_key = dim_lookup.lookup_subcatalogs_df[dim_lookup.lookup_subcatalogs_df['SubCatalogName'] == subcatalog_name].get('SubCatalogId').values[0]
    return subcatalog_key


from jobs.utils.nlp_utils import map_to_processed_tokens
from gensim.models import LdaMulticore
def map_product_usage_instruction_to_fkey(usage_instruction: str) -> int:
    """
    Load LDA model to get the topic fkey for the usage instruction
    """
    usage_instruction_processed_tokens = map_to_processed_tokens(usage_instruction)
    # no tokens after preprocessing, so if we  still use lda, it will give the wrong result
    if usage_instruction_processed_tokens.count == 0 or usage_instruction_processed_tokens is None or usage_instruction_processed_tokens is []:
        return -1
    trained_lda_model:LdaMulticore = LdaMulticore.load('resources/lda_usage_instruction.model')
    usage_instruction_processed_tokens_bow = trained_lda_model.id2word.doc2bow(usage_instruction_processed_tokens)
    # print(usage_instruction_processed_tokens_bow)
    usage_instruction_topic_fkey = trained_lda_model[usage_instruction_processed_tokens_bow]
    # print(usage_instruction_topic_fkey) to knoww what it looks like
    usage_instruction_topic_fkey = sorted(usage_instruction_topic_fkey[0], key=lambda x: x[1], reverse=True)[0][0]
    # sort by the probability and get the first one (the one with highest probability)
    return usage_instruction_topic_fkey

def map_product_preserve_instruction_to_fkey(preserve_instruction: str) -> int:
    """
    Load LDA model to get the topic fkey for the preserve instruction
    """
    preserve_instruction_processed_tokens = map_to_processed_tokens(preserve_instruction)
    # no tokens after preprocessing, so if we  still use lda, it will give the wrong result
    if preserve_instruction_processed_tokens.count == 0 or preserve_instruction_processed_tokens is None or preserve_instruction_processed_tokens is []:
        return -1
    trained_lda_model:LdaMulticore = LdaMulticore.load('resources/lda_preserve_instruction.model')
    preserve_instruction_processed_tokens_bow = trained_lda_model.id2word.doc2bow(preserve_instruction_processed_tokens)
    preserve_instruction_topic_fkey = trained_lda_model[preserve_instruction_processed_tokens_bow]
    preserve_instruction_topic_fkey = sorted(preserve_instruction_topic_fkey[0], key=lambda x: x[1], reverse=True)[0][0]
    return preserve_instruction_topic_fkey