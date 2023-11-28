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
        if product_usage_instruction not in usage_instruction_dict.values():
            usage_instruction_dict[idx] = product_usage_instruction
            idx += 1

    # print(brand_dict.values())
    df = pd.DataFrame.from_dict(usage_instruction_dict, orient='index', columns=['ProductUsageInstruction'])
    df.to_csv('./resources/product_usage_instruction.csv', index=False)
    db.client.close()
    logging.log(logging.INFO, 'Connection to MongoDB closed.')
    return df