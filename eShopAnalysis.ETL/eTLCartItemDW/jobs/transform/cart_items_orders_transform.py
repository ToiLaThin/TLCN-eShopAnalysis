import pandas as pd
def transform_cart_item_df_to_cart_item_df_with_fkey(cart_item_df: pd.DataFrame):
    cart_item_df_with_fkey = cart_item_df.copy()
    cart_item_df_with_fkey['DateStockConfirmedKey'] = cart_item_df['DateStockConfirmed'].dt.strftime('%Y%m%d%H%M')
    # cart_item_df_with_fkey['DateStockConfirmedKey'] = cart_item_df['DateStockConfirmed'].dt.date
    cart_item_df_with_fkey.drop(columns=['DateStockConfirmed'], inplace=True)
    print(cart_item_df_with_fkey.head(5))
    return cart_item_df_with_fkey