import os
from dotenv import load_dotenv
import pymongo
import pyodbc
import logging
import traceback

load_dotenv('config.env')

def get_cursor_data_dw_mssql():
    """Get cursor for MSSQL database data warehouse"""
    logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
    MSSQL_SERVER = os.environ.get('MSSQL_SERVER')
    MSSQL_DW_DATABASE = os.environ.get('MSSQL_DW_DATABASE')
    CONNECTION_STRING = 'DRIVER={ODBC Driver 17 for SQL Server};SERVER=' + MSSQL_SERVER + ';DATABASE=' + MSSQL_DW_DATABASE + ';Trusted_Connection=yes;'
    try:
        conn = pyodbc.connect(CONNECTION_STRING)
        cursor = conn.cursor()
        logging.info('Connected to MSSQL database')
        return cursor
    except Exception as e:
        logging.error('Error connecting to MSSQL database')
        logging.error(traceback.format_exc())


def get_db_mongo():
    """Get cursor for MongoDB source database"""
    logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
    MONGO_CONNECTION_STRING = os.environ.get('MONGO_CONNECTION_STRING')
    MONGO_DATABASE = os.environ.get('MONGO_DATABASE')
    try:
        conn = pymongo.MongoClient(MONGO_CONNECTION_STRING)
        db = conn.get_database(MONGO_DATABASE)
        logging.info('Connected to MongoDB database')
        return db
    except Exception as e:
        logging.error('Error connecting to MongoDB database')
        logging.error(traceback.format_exc())

def get_mssql_data_src_cursor():
    """Get cursor for MSSQL database data source"""
    logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
    MSSQL_SERVER = os.environ.get('MSSQL_SERVER')
    MSSQL_SRC_DATABASE = os.environ.get('MSSQL_SRC_DATABASE')
    CONNECTION_STRING = 'DRIVER={ODBC Driver 17 for SQL Server};SERVER=' + MSSQL_SERVER + ';DATABASE=' + MSSQL_SRC_DATABASE + ';Trusted_Connection=yes;'
    try:
        conn = pyodbc.connect(CONNECTION_STRING)
        cursor = conn.cursor()
        logging.info('Connected to MSSQL database')
        return cursor
    except Exception as e:
        logging.error('Error connecting to MSSQL database')
        logging.error(traceback.format_exc())