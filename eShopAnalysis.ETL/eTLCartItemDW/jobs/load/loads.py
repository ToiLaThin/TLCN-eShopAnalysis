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

def load_usage_instruction_df_to_mssql(reduced_usage_instruction_df: pd.DataFrame):
    mssql_cursor = get_cursor_mssql()
    usage_instruction_table_name = os.environ.get('MSSQL_DIM_USAGE_INSTRUCTION_TABLE_NAME')

    mssql_cursor.execute(f"""DELETE FROM {usage_instruction_table_name}""")
    mssql_cursor.commit()

    for index, row in reduced_usage_instruction_df.iterrows():
        usage_instruction_type_id = index
        usage_instruction_type_name = row['UsageInstructionTypeName']
        mssql_cursor.execute(f"""INSERT INTO {usage_instruction_table_name} (UsageInstructionTypeId, UsageInstructionTypeName) VALUES (?, ?)""", usage_instruction_type_id, usage_instruction_type_name)
    mssql_cursor.commit()

    query = f'SELECT * FROM {usage_instruction_table_name}'
    df_usage_instruction = pd.read_sql_query(query, mssql_cursor.connection)
    mssql_cursor.close()
    logging.log(logging.INFO, 'Connection to MSSQL closed.')
    print(df_usage_instruction)

def load_preserve_instruction_df_to_mssql(reduced_preserve_instruction_df: pd.DataFrame):
    mssql_cursor = get_cursor_mssql()
    preserve_instruction_table_name = os.environ.get('MSSQL_DIM_PRESERVE_INSTRUCTION_TABLE_NAME')

    mssql_cursor.execute(f"""DELETE FROM {preserve_instruction_table_name}""")
    mssql_cursor.commit()

    for index, row in reduced_preserve_instruction_df.iterrows():
        preserve_instruction_type_id = index
        preserve_instruction_type_name = row['PreserveInstructionTypeName']
        mssql_cursor.execute(f"""INSERT INTO {preserve_instruction_table_name} (PreserveInstructionTypeId, PreserveInstructionTypeName) VALUES (?, ?)""", preserve_instruction_type_id, preserve_instruction_type_name)
    mssql_cursor.commit()

    query = f'SELECT * FROM {preserve_instruction_table_name}'
    df_preserve_instruction = pd.read_sql_query(query, mssql_cursor.connection)
    mssql_cursor.close()
    logging.log(logging.INFO, 'Connection to MSSQL closed.')
    print(df_preserve_instruction)

def load_product_df_to_mssql(product_df: pd.DataFrame):
    mssql_cursor = get_cursor_mssql()
    product_table_name = os.environ.get('MSSQL_DIM_PRODUCT_TABLE_NAME')

    mssql_cursor.execute(f"""DELETE FROM {product_table_name}""")
    mssql_cursor.commit()

    for _, row in product_df.iterrows():
        product_id = row['ProductId']
        product_name = row['ProductName']
        brand_fkey = row['BrandKey']
        usage_instruction_type_fkey = row['UsageInstructionTypeKey']
        subcatalog_fkey = row['SubCatalogKey']
        preserve_instruction_type_fkey = row['PreserveInstructionTypeKey']
        have_models = row['HaveModels']
        have_price_per_cublic = row['HavePricePerCublic']
        business_key = row['BusinessKey']
        revision = row['Revision']
        mssql_cursor.execute(f"""
            INSERT INTO {product_table_name} 
            (ProductId, ProductName, BrandKey, UsageInstructionTypeKey, 
            SubCatalogKey, PreserveInstructionTypeKey, HaveModels, 
            HavePricePerCublic, BusinessKey, Revision) 
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)""", 
            product_id, product_name, brand_fkey, usage_instruction_type_fkey, subcatalog_fkey, 
            preserve_instruction_type_fkey, have_models, have_price_per_cublic, business_key, revision)
    mssql_cursor.commit()

    query = f'SELECT * FROM {product_table_name}'
    df_products = pd.read_sql_query(query, mssql_cursor.connection)
    mssql_cursor.close()
    logging.log(logging.INFO, 'Connection to MSSQL closed.')
    print(df_products)

def load_product_model_df_to_mssql(product_model_df: pd.DataFrame):
    mssql_cursor = get_cursor_mssql()
    product_model_table_name = os.environ.get('MSSQL_DIM_PRODUCT_MODEL_TABLE_NAME')

    mssql_cursor.execute(f"""DELETE FROM {product_model_table_name}""")
    mssql_cursor.commit()

    for _, row in product_model_df.iterrows():
        product_model_id = row['ProductModelId']
        product_model_name = row['ProductModelName']
        business_key = row['BusinessKey']
        product_fkey = row['ProductId']
        cublic_type_fkey = row['CublicTypeKey']
        mssql_cursor.execute(f"""
            INSERT INTO {product_model_table_name} 
            (ProductModelId, ProductModelName, BusinessKey, ProductKey, CublicTypeKey) 
            VALUES (?, ?, ?, ?, ?)""", 
            product_model_id, product_model_name, business_key, product_fkey, cublic_type_fkey)
    mssql_cursor.commit()

    query = f'SELECT * FROM {product_model_table_name}'
    df_product_models = pd.read_sql_query(query, mssql_cursor.connection)
    mssql_cursor.close()
    logging.log(logging.INFO, 'Connection to MSSQL closed.')
    print(df_product_models)