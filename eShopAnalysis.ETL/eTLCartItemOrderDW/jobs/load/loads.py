import pandas as pd
from shared.get_connections import *
from shared.queries import *
load_dotenv('config.env')

def load_brand_df_to_mssql(brand_df: pd.DataFrame):
    mssql_cursor = get_cursor_data_dw_mssql()
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
    mssql_cursor = get_cursor_data_dw_mssql()
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
    mssql_cursor = get_cursor_data_dw_mssql()
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
    mssql_cursor = get_cursor_data_dw_mssql()
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
    mssql_cursor = get_cursor_data_dw_mssql()
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
    mssql_cursor = get_cursor_data_dw_mssql()
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
    mssql_cursor = get_cursor_data_dw_mssql()
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

def load_date_df_to_mssql(date_df: pd.DataFrame):
    mssql_cursor = get_cursor_data_dw_mssql()
    date_table_name = os.environ.get('MSSQL_DIM_DATE_TABLE_NAME')

    mssql_cursor.execute(f"""DELETE FROM {date_table_name}""")
    mssql_cursor.commit()

    for _, row in date_df.iterrows():
        date_key = row['DateKey']
        date = row['Date']
        year = row['Year']
        month = row['Month']
        quarter = row['Quarter']
        day = row['Day']
        weekday = row['Weekday']
        weekday_name = row['WeekdayName']
        mssql_cursor.execute(f"""
            INSERT INTO {date_table_name} (DateKey, Date, Year, Quarter, Month, Day, Weekday, WeekdayName) 
            VALUES (?, ?, ?, ?, ?, ?, ?, ?)""",date_key, date, year, quarter, month, day, weekday, weekday_name)
    mssql_cursor.commit()

    query = f'SELECT TOP 10 * FROM {date_table_name}'
    df_date = pd.read_sql_query(query, mssql_cursor.connection)
    mssql_cursor.close()
    logging.log(logging.INFO, 'Connection to MSSQL closed.')
    print(df_date)

def load_cart_item_df_to_mssql(cart_item_df: pd.DataFrame):
    mssql_cursor = get_cursor_data_dw_mssql()
    cart_item_table_name = os.environ.get('MSSQL_FACT_CART_ITEM_TABLE_NAME')

    mssql_cursor.execute(f"""DELETE FROM {cart_item_table_name}""")
    mssql_cursor.commit()

    for _, row in cart_item_df.iterrows():
        cart_id = row['CartId']
        cart_item_product_id = row['ProductId']
        cart_item_product_model_id = row['ProductModelId']
        cart_item_business_key = row['BusinessKey']
        cart_item_sale_item_id = row['SaleItemId']
        cart_item_is_on_sale = row['IsOnSale']
        cart_item_sale_value = row['SaleValue']
        cart_item_quantity = row['Quantity']
        cart_item_unit_price = row['UnitPrice']
        cart_item_final_price = row['FinalPrice']
        cart_item_unit_after_sale_price = row['UnitAfterSalePrice']
        cart_item_final_after_sale_price = row['FinalAfterSalePrice']
        cart_item_date_stock_confirmed_key = row['DateStockConfirmedKey']

        mssql_cursor.execute(f"""
            INSERT INTO {cart_item_table_name} 
            (CartId, ProductId, ProductModelId, 
            BusinessKey, SaleItemKey, IsOnSale, 
            SaleValue, Quantity, UnitPrice, FinalPrice, 
            UnitAfterSalePrice, FinalAfterSalePrice, DateStockConfirmedKey) 
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)"""\
                , cart_id, cart_item_product_id, cart_item_product_model_id\
                , cart_item_business_key, cart_item_sale_item_id, cart_item_is_on_sale\
                , cart_item_sale_value, cart_item_quantity\
                , cart_item_unit_price, cart_item_final_price, cart_item_unit_after_sale_price, cart_item_final_after_sale_price\
                , cart_item_date_stock_confirmed_key
        )
        mssql_cursor.commit()
        
    query = f'SELECT * FROM {cart_item_table_name}'
    df_cart_items = pd.read_sql_query(query, mssql_cursor.connection)
    mssql_cursor.close()
    logging.log(logging.INFO, 'Connection to MSSQL closed.')
    print(df_cart_items)


def load_payment_method_df_to_mssql(payment_method_df: pd.DataFrame):
    mssql_cursor = get_cursor_data_dw_mssql()
    payment_method_table_name = os.environ.get('MSSQL_DIM_PAYMENT_METHOD_TABLE_NAME')

    mssql_cursor.execute(f"""DELETE FROM {payment_method_table_name}""")
    mssql_cursor.commit()

    for id, row in payment_method_df.iterrows():
        payment_method_key = id
        payment_method_name = row['PaymentMethod']
        mssql_cursor.execute(f"""
            INSERT INTO {payment_method_table_name} (PaymentMethodId, MethodName) VALUES (?, ?)
        """, 
        payment_method_key, 
        payment_method_name)
    mssql_cursor.commit()

    query = f'SELECT * FROM {payment_method_table_name}'
    df_payment_methods = pd.read_sql_query(query, mssql_cursor.connection)
    mssql_cursor.close()
    logging.log(logging.INFO, 'Connection to MSSQL closed.')
    print(df_payment_methods)

def load_discount_type_df_to_mssql(discount_type_df: pd.DataFrame):
    mssql_cursor = get_cursor_data_dw_mssql()
    discount_type_table_name = os.environ.get('MSSQL_DIM_DISCOUNT_TYPE_TABLE_NAME')

    mssql_cursor.execute(f"""DELETE FROM {discount_type_table_name}""")
    mssql_cursor.commit()

    for id, row in discount_type_df.iterrows():
        discount_type_key = id
        discount_type_name = row['DiscountType']
        mssql_cursor.execute(f"""
            INSERT INTO {discount_type_table_name} (DiscountTypeId, DiscountTypeName) VALUES (?, ?)
        """, 
        discount_type_key, 
        discount_type_name)
    mssql_cursor.commit()

    query = f'SELECT * FROM {discount_type_table_name}'
    df_discount_types = pd.read_sql_query(query, mssql_cursor.connection)
    mssql_cursor.close()
    logging.log(logging.INFO, 'Connection to MSSQL closed.')
    print(df_discount_types)

def load_order_status_df_to_mssql(order_status_df: pd.DataFrame):
    mssql_cursor = get_cursor_data_dw_mssql()
    order_status_table_name = os.environ.get('MSSQL_DIM_ORDER_STATUS_TABLE_NAME')

    mssql_cursor.execute(f"""DELETE FROM {order_status_table_name}""")
    mssql_cursor.commit()

    for id, row in order_status_df.iterrows():
        order_status_key = id
        order_status_name = row['OrderStatus']
        mssql_cursor.execute(f"""
            INSERT INTO {order_status_table_name} (OrderStatusId, OrderStatusName) VALUES (?, ?)
        """, 
        order_status_key, 
        order_status_name)
    mssql_cursor.commit()

    query = f'SELECT * FROM {order_status_table_name}'
    df_order_statuses = pd.read_sql_query(query, mssql_cursor.connection)
    mssql_cursor.close()
    logging.log(logging.INFO, 'Connection to MSSQL closed.')
    print(df_order_statuses)

def load_address_df_to_mssql(address_df: pd.DataFrame):
    mssql_cursor = get_cursor_data_dw_mssql()
    address_table_name = os.environ.get('MSSQL_DIM_ADDRESS_TABLE_NAME')

    mssql_cursor.execute(f"""DELETE FROM {address_table_name}""")
    mssql_cursor.commit()

    for id, row in address_df.iterrows():
        address_key = id
        district_or_locality = row['Address']
        mssql_cursor.execute(f"""
            INSERT INTO {address_table_name} (AddressId, DistrictOrLocality) VALUES (?, ?)
        """, 
        address_key, 
        district_or_locality)
    mssql_cursor.commit()

    query = f'SELECT * FROM {address_table_name}'
    df_addresses = pd.read_sql_query(query, mssql_cursor.connection)
    mssql_cursor.close()
    logging.log(logging.INFO, 'Connection to MSSQL closed.')
    print(df_addresses)

def load_cart_order_df_to_mssql(cart_order_df: pd.DataFrame):
    mssql_cursor = get_cursor_data_dw_mssql()
    cart_order_table_name = os.environ.get('MSSQL_FACT_CART_ORDER_TABLE_NAME')

    mssql_cursor.execute(f"""DELETE FROM {cart_order_table_name}""")
    mssql_cursor.commit()

    for _, row in cart_order_df.iterrows():
        cart_order_id = row['Id']
        date_created_draft_key = row['DateCreatedDraftKey']
        date_stock_confirmed_key = row['DateStockConfirmedKey']
        orders_status_key = row['OrdersStatusKey']
        payment_method_key = row['PaymentMethodKey']
        discount_type_key = row['DiscountTypeKey']
        address_key = row['AddressKey']

        total_sale_discount_amount = row['TotalSaleDiscountAmount']
        coupon_discount_amount = row['CouponDiscountAmount']
        coupon_discount_value = row['CouponDiscountValue']
        total_price_original = row['TotalPriceOriginal']
        total_price_after_sale = row['TotalPriceAfterSale']
        total_price_after_coupon_applied = row['TotalPriceAfterCouponApplied']
        total_price_final = row['TotalPriceFinal']
        time_lag_to_approve = row['TimeLagToApprove']

        have_coupon_applied = row['HaveCouponApplied']
        have_any_sale_item = row['HaveAnySaleItem']
        
        mssql_cursor.execute(f"""
            INSERT INTO {cart_order_table_name} 
            (CartOrderId, AddressKey, PaymentMethodKey, OrderStatusKey, DiscountTypeKey, DateCreatedDraftKey, DateStockConfirmedKey,
            HaveCouponApplied, HaveAnySaleItem, CouponDiscountAmount, CouponDiscountValue, TotalSaleDiscountAmount, TotalPriceOriginal, 
            TotalPriceAfterSale, TotalPriceAfterCouponApplied, TotalPriceFinal, TimeLagToApprove) 
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)""",
            cart_order_id, 
            address_key, 
            payment_method_key, 
            orders_status_key, 
            discount_type_key, 
            date_created_draft_key, 
            date_stock_confirmed_key,
            have_coupon_applied,
            have_any_sale_item,
            coupon_discount_amount,
            coupon_discount_value,
            total_sale_discount_amount,
            total_price_original,
            total_price_after_sale,
            total_price_after_coupon_applied,
            total_price_final,
            time_lag_to_approve
        )
        mssql_cursor.commit()