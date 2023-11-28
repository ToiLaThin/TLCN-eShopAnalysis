import pandas as pd
from shared.get_connections import *
from shared.queries import *
load_dotenv('config.env')

def load_brand_df_to_mssql(brand_df: pd.DataFrame):
    mssql_cursor = get_cursor_mssql()
    brand_table_name = os.environ.get('MSSQL_DIM_BRAND_TABLE_NAME')

    mssql_cursor.execute(f"""DELETE FROM {brand_table_name}""")
    mssql_cursor.commit()

    for index, row in brand_df.iterrows():
        brand_name = row['ProductBrand']
        mssql_cursor.execute(f"""INSERT INTO {brand_table_name} VALUES (?, ?)""", index, brand_name)
    mssql_cursor.commit()

    query = f'SELECT * FROM {brand_table_name}'
    df_brands = pd.read_sql_query(query, mssql_cursor.connection)
    mssql_cursor.close()
    logging.log(logging.INFO, 'Connection to MSSQL closed.')
    # print(df_brands)

def load_catalog_df_to_mssql(catalog_df: pd.DataFrame):
    mssql_cursor = get_cursor_mssql()
    catalog_table_name = os.environ.get('MSSQL_DIM_CATALOG_TABLE_NAME')

    mssql_cursor.execute(f"""DELETE FROM {catalog_table_name}""")
    mssql_cursor.commit()

    for _, row in catalog_df.iterrows():
        catalog_name = row['CatalogName']
        catalog_id = row['CatalogId']
        mssql_cursor.execute(f"""INSERT INTO {catalog_table_name} (CatalogId, CatalogName) VALUES (?, ?)""", catalog_id, catalog_name)
    mssql_cursor.commit()

    query = f'SELECT * FROM {catalog_table_name}'
    df_catalogs = pd.read_sql_query(query, mssql_cursor.connection)
    mssql_cursor.close()
    logging.log(logging.INFO, 'Connection to MSSQL closed.')
    print(df_catalogs)

def load_subcatalog_df_to_mssql(subcatalog_df: pd.DataFrame):
    mssql_cursor = get_cursor_mssql()
    subcatalog_table_name = os.environ.get('MSSQL_DIM_SUBCATALOG_TABLE_NAME')

    mssql_cursor.execute(f"""DELETE FROM {subcatalog_table_name}""")
    mssql_cursor.commit()

    for _, row in subcatalog_df.iterrows():
        subcatalog_name = row['SubCatalogName']
        subcatalog_id = row['SubCatalogId']
        catalog_id = row['CatalogId']
        mssql_cursor.execute(f"""INSERT INTO {subcatalog_table_name} (SubCatalogId, SubCatalogName, CatalogKey) VALUES (?, ?, ?)""", subcatalog_id, subcatalog_name, catalog_id)
    mssql_cursor.commit()

    query = f'SELECT * FROM {subcatalog_table_name}'
    df_subcatalogs = pd.read_sql_query(query, mssql_cursor.connection)
    mssql_cursor.close()
    logging.log(logging.INFO, 'Connection to MSSQL closed.')
    print(df_subcatalogs)