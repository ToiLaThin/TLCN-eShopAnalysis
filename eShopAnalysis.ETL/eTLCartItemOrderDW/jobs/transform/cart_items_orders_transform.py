import pandas as pd
def transform_cart_item_df_to_cart_item_df_with_fkey(cart_item_df: pd.DataFrame):
    cart_item_df_with_fkey = cart_item_df.copy()
    cart_item_df_with_fkey['DateStockConfirmedKey'] = cart_item_df['DateStockConfirmed'].dt.strftime('%Y%m%d%H%M')
    # cart_item_df_with_fkey['DateStockConfirmedKey'] = cart_item_df['DateStockConfirmed'].dt.date
    cart_item_df_with_fkey.drop(columns=['DateStockConfirmed'], inplace=True)
    print(cart_item_df_with_fkey.head(5))
    return cart_item_df_with_fkey

def transform_cart_order_df_to_cart_order_df_with_fkey(cart_order_df: pd.DataFrame):
    # DateCreatedDraft	DateCheckouted	DateCustomerInfoConfirmed	DateStockConfirmed	DateCompleted	DateCancelled	DateRefunded
    cart_order_df_with_fkey = cart_order_df.copy()
    cart_order_df_with_fkey['DateStockConfirmedKey'] = cart_order_df_with_fkey['DateStockConfirmed'].dt.strftime('%Y%m%d%H%M')
    cart_order_df_with_fkey['DateCreatedDraftKey'] = cart_order_df_with_fkey['DateCreatedDraft'].dt.strftime('%Y%m%d%H%M')
    cart_order_df_with_fkey['TimeLagToApprove'] = (cart_order_df_with_fkey['DateStockConfirmed'] - cart_order_df_with_fkey['DateCreatedDraft']).dt.total_seconds() / 3600

    # the enum must match with the enum in backend, the OrdersStatus type is int is the key too already
    cart_order_df_with_fkey['OrdersStatusKey'] = cart_order_df['OrdersStatus'].apply(lambda x: x)
    cart_order_df_with_fkey['PaymentMethodKey'] = cart_order_df['PaymentMethod'].apply(lambda x: x)
    cart_order_df_with_fkey['DiscountTypeKey'] = cart_order_df['CouponDiscountType'].apply(lambda x: 2 if x is None else x) # 2 is NoDiscount, later if we have more discount type, we will change this, beware of the enum in backend

    cart_order_df_with_fkey['TotalSaleDiscountAmount'] =cart_order_df_with_fkey['TotalSaleDiscountAmount'].apply(lambda x: 0 if x == -1 else x)
    cart_order_df_with_fkey['CouponDiscountAmount'] = cart_order_df_with_fkey['CouponDiscountAmount'].apply(lambda x: 0 if x == -1 else x)
    cart_order_df_with_fkey['CouponDiscountValue'] = cart_order_df_with_fkey['CouponDiscountValue'].apply(lambda x: 0 if x == -1 else x)
    cart_order_df_with_fkey['TotalPriceAfterSale'] = cart_order_df_with_fkey['TotalPriceOriginal'] - cart_order_df_with_fkey['TotalSaleDiscountAmount']
    cart_order_df_with_fkey['TotalPriceAfterCouponApplied'] = cart_order_df_with_fkey['TotalPriceAfterSale'] - cart_order_df_with_fkey['CouponDiscountAmount']
    cart_order_df_with_fkey['TotalPriceFinal'] = cart_order_df_with_fkey['TotalPriceAfterCouponApplied']

    from random import choice
    from jobs.utils.enum_utils import Address
    addresses = [Address.Quan1, Address.QuanThuDuc, Address.QuanBinhThanh, Address.QuanTanBinh]
    cart_order_df_with_fkey['AddressKey'] = cart_order_df_with_fkey['Id'].apply(lambda x: choice(addresses).value[0] if type(choice(addresses).value) is tuple else choice(addresses).value)

    drop_columns = ['DateCompleted', 'DateCancelled', 'DateRefunded', 'DateStockConfirmed', 'DateCreatedDraft', 'DateCheckouted', 'DateCustomerInfoConfirmed',\
                    'OrdersStatus', 'PaymentMethod', 'CouponDiscountType']
    
    cart_order_df_with_fkey.drop(columns=drop_columns, inplace=True)
    return cart_order_df_with_fkey