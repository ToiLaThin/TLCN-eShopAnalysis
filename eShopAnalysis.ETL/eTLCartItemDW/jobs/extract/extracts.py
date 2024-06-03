import pandas as pd
from shared.get_connections import *
load_dotenv('config.env')

def extract_mongo_brands_to_df() -> pd.DataFrame:
    MONGODB_PRODUCT_COLLECTION = os.environ.get('MONGODB_PRODUCT_COLLECTION')
    db = get_db_mongo()
    product_collection = db.get_collection(MONGODB_PRODUCT_COLLECTION)
    
    cursor_products = product_collection.find({})
    idx = 0
    brand_dict: dict = {
    }
    for p in cursor_products:
        product_brand_type = p.get('ProductInfo').get('ProductBrand')
        if product_brand_type not in brand_dict.values():
            brand_dict[idx] = product_brand_type
            idx += 1

    # print(brand_dict.values())
    df = pd.DataFrame.from_dict(brand_dict, orient='index', columns=['ProductBrand'])
    df.to_csv('./resources/product_brand.csv', index=False)
    db.client.close()
    logging.log(logging.INFO, 'Connection to MongoDB closed.')
    return df

def extract_mongo_catalogs_to_df() -> pd.DataFrame:
    MONGODB_CATALOG_COLLECTION = os.environ.get('MONGODB_CATALOG_COLLECTION')
    db = get_db_mongo()
    catalog_collection = db.get_collection(MONGODB_CATALOG_COLLECTION)

    cursor_catalogs = catalog_collection.find({})
    df_catalogs_with_subcatalog_list = pd.DataFrame(list(cursor_catalogs))
    df_catalogs_only = df_catalogs_with_subcatalog_list[['CatalogName', '_id']]
    df_catalogs_only.rename(columns={'_id': 'CatalogId'}, inplace=True)
    df_catalogs_only.to_csv('./resources/catalog.csv', index=False)
    return df_catalogs_only

def extract_mongo_subcatalogs_to_df() -> pd.DataFrame:    
    MONGODB_CATALOG_COLLECTION = os.environ.get('MONGODB_CATALOG_COLLECTION')
    db = get_db_mongo()
    catalog_collection = db.get_collection(MONGODB_CATALOG_COLLECTION)
    count_catalog_without_subcatalog = catalog_collection.count_documents({'CatalogSubCatalogs': []})
    print(f'Catalog without any subcatalog: {count_catalog_without_subcatalog}')
    cursor_catalogs = catalog_collection.find({})
    idx = 0
    subcatalog_dict = {}
    for c in cursor_catalogs:
        catalog_subcatalogs = c.get('CatalogSubCatalogs')
        if catalog_subcatalogs is None or catalog_subcatalogs == []:
            continue
        catalog_id = c.get('_id')
        for subcatalog in catalog_subcatalogs:
            subcatalog_id = subcatalog.get('_id')
            subcatalog_name = subcatalog.get('SubCatalogName')
            subcatalog_dict[idx] = {
                'SubCatalogId': subcatalog_id, 
                'SubCatalogName': subcatalog_name,
                'CatalogId': catalog_id
            }
            idx += 1
    df_subcatalog_with_catalog_id = pd.DataFrame.from_dict(subcatalog_dict, orient='index')
    df_subcatalog_with_catalog_id.to_csv('./resources/subcatalog.csv', index=False)
    print(df_subcatalog_with_catalog_id.info())
    print(df_subcatalog_with_catalog_id.value_counts())
    return df_subcatalog_with_catalog_id

def extract_mongo_usage_instruction_to_df() -> pd.DataFrame:
    MONGODB_PRODUCT_COLLECTION = os.environ.get('MONGODB_PRODUCT_COLLECTION')
    db = get_db_mongo()
    product_collection = db.get_collection(MONGODB_PRODUCT_COLLECTION)
    
    cursor_products = product_collection.find({})
    idx = 0
    usage_instruction_dict: dict = {
    }
    for p in cursor_products:
        product_usage_instruction = p.get('ProductInfo').get('ProductUsageInstruction')
        # TODO: if statement affect nlp or not?
        if product_usage_instruction not in usage_instruction_dict.values():
            usage_instruction_dict[idx] = product_usage_instruction
            idx += 1

    # print(brand_dict.values())
    df = pd.DataFrame.from_dict(usage_instruction_dict, orient='index', columns=['ProductUsageInstruction'])
    df.to_csv('./resources/product_usage_instruction.csv', index=False)
    db.client.close()
    logging.log(logging.INFO, 'Connection to MongoDB closed.')
    return df

def extract_mongo_preserve_instruction_to_df() -> pd.DataFrame:
    MONGODB_PRODUCT_COLLECTION = os.environ.get('MONGODB_PRODUCT_COLLECTION')
    db = get_db_mongo()
    product_collection = db.get_collection(MONGODB_PRODUCT_COLLECTION)
    
    cursor_products = product_collection.find({})
    idx = 0
    preserve_instruction_dict: dict = {
    }
    for p in cursor_products:
        product_preserve_instruction = p.get('ProductInfo').get('ProductPreserveInstruction')
        # TODO: if statement affect nlp or not?
        # if product_preserve_instruction not in preserve_instruction_dict.values():
        preserve_instruction_dict[idx] = product_preserve_instruction
        idx += 1

    # print(brand_dict.values())
    df = pd.DataFrame.from_dict(preserve_instruction_dict, orient='index', columns=['PreserveInstructionTypeName'])
    df.to_csv('./resources/product_preserve_instruction.csv', index=False)
    db.client.close()
    logging.log(logging.INFO, 'Connection to MongoDB closed.')
    return df

def extract_mongo_product_to_df() -> pd.DataFrame:
    MONGODB_PRODUCT_COLLECTION = os.environ.get('MONGODB_PRODUCT_COLLECTION')
    db = get_db_mongo()
    product_collection = db.get_collection(MONGODB_PRODUCT_COLLECTION)

    cursor_products = product_collection.find({})
    # create an empty dataframe, previousely we create df from dict 
    # but this time there are more columns, and it's more complex, so we use Series and concat
    return_df = pd.DataFrame()
    for p in cursor_products:
        try:
            # they must match the prop in the mongo collection
            product_id = p.get('_id')
            product_name = p.get('ProductName')
            product_brand_name = p.get('ProductInfo').get('ProductBrand')
            product_subcatalog_name = p.get('SubCatalogName')
            product_preserve_instruction = p.get('ProductInfo').get('ProductPreserveInstruction')
            product_usage_instruction = p.get('ProductInfo').get('ProductUsageInstruction')
            have_models = p.get('HaveVariants')
            have_price_per_cublic = p.get('HavePricePerCublic')
            product_business_key = p.get('BusinessKey')
            product_revision = p.get('Revision')


            df_row_from_series = pd.Series([product_id, product_name, product_brand_name, product_subcatalog_name\
                                            , product_preserve_instruction, product_usage_instruction, have_models\
                                            , have_price_per_cublic, product_business_key, product_revision])\
                                        .to_frame().T
            return_df = pd.concat([return_df, df_row_from_series], ignore_index=True)
        except Exception as e:
            print(e)
            continue
    db.client.close()
    logging.log(logging.INFO, 'Connection to MongoDB closed.')
    return_df.columns = ['ProductId', 'ProductName', 'ProductBrandName'\
                        , 'ProductSubCatalogName', 'ProductPreserveInstruction', 'ProductUsageInstruction'\
                        , 'HaveModels', 'HavePricePerCublic', 'BusinessKey', 'Revision']
    return_df.to_csv('./resources/product_not_transformed_yet.csv', index=False)
    return return_df

def extract_mongo_product_model_to_df() -> pd.DataFrame:
    MONGODB_PRODUCT_COLLECTION = os.environ.get('MONGODB_PRODUCT_COLLECTION')
    db = get_db_mongo()
    product_collection = db.get_collection(MONGODB_PRODUCT_COLLECTION)

    cursor_products = product_collection.find({})
    return_df = pd.DataFrame()
    for p in cursor_products:
        try:
            product_models = p.get('ProductModels')
            product_model_id = product_models[0].get('_id')
            product_model_name = p.get('ProductName')
            product_business_key = p.get('BusinessKey')
            product_id = p.get('_id')
            product_model_cublic_type_fkey = product_models[0].get('CublicType')

            df_row_from_series = pd.Series([product_model_id, product_model_name, product_business_key\
                                            , product_id, product_model_cublic_type_fkey])\
                                        .to_frame().T
            return_df = pd.concat([return_df, df_row_from_series], ignore_index=True)
        except Exception as e:
            print(e)
            continue
    db.client.close()
    logging.log(logging.INFO, 'Connection to MongoDB closed.')
    return_df.columns = ['ProductModelId', 'ProductModelName', 'BusinessKey', 'ProductId', 'CublicTypeKey']
    return_df.to_csv('./resources/product_model.csv', index=False)
    return return_df

def extract_mssql_cart_item_to_df() -> pd.DataFrame:
    MSSQL_SRC_CART_ITEM_TABLE_NAME = os.environ.get('MSSQL_SRC_CART_ITEM_TABLE_NAME')
    MSSQL_SRC_CART_TABLE_NAME = os.environ.get('MSSQL_SRC_CART_TABLE_NAME')
    MSSQL_SRC_ORDER_TABLE_NAME = os.environ.get('MSSQL_SRC_ORDER_TABLE_NAME')
    mssql_data_src_cursor = get_mssql_data_src_cursor()    
    query = f"""
        SELECT cI.CartId, cI.ProductId, cI.ProductModelId, cI.BusinessKey\
                       , cI.SaleItemId, cI.IsOnSale, cI.SaleValue, cI.Quantity, cI.UnitPrice\
                       , cI.FinalPrice, cI.UnitAfterSalePrice, cI.FinalAfterSalePrice, o.DateStockConfirmed 
        FROM {MSSQL_SRC_CART_ITEM_TABLE_NAME} cI
        INNER JOIN {MSSQL_SRC_CART_TABLE_NAME} c ON c.Id = cI.CartId
        INNER JOIN {MSSQL_SRC_ORDER_TABLE_NAME} o ON c.Id = o.CartId
        WHERE o.OrdersStatus = 3
    """
    df_cart_item = pd.read_sql_query(query, mssql_data_src_cursor.connection)
    mssql_data_src_cursor.close()
    logging.log(logging.INFO, 'Connection to MSSQL closed.')

    # this can be in transform
    columns_to_keep = ['CartId', 'ProductId', 'ProductModelId', 'BusinessKey'\
                       , 'SaleItemId', 'IsOnSale', 'SaleValue', 'Quantity', 'UnitPrice'\
                       , 'FinalPrice', 'UnitAfterSalePrice', 'FinalAfterSalePrice', 'DateStockConfirmed']
    df_cart_item = df_cart_item[columns_to_keep]
    return df_cart_item