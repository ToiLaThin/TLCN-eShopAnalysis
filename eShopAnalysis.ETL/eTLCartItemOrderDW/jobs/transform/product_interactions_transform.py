import pandas as pd

def transform_product_user_interactions_df(interactions_df: pd.DataFrame) -> pd.DataFrame:
    interactions_df.drop(columns=['_id', '_id_x', '_id_y'], inplace=True)
    interactions_df.rename(
        columns={
            'ProductBusinessKey': 'product_key', 
            'UserId': 'user_key',
            'Rating': 'rating',
            'Status': 'liked',
            'Bookmark': 'bookmarked',
        }, 
        inplace=True
    )
    interactions_df['liked'] = interactions_df['liked'].apply(lambda x: 1 if x == 0 else 0) #0 là liked, NaN là không liked (neutral)
    interactions_df['bookmarked'] = interactions_df['liked'].apply(lambda x: 1 if x == 1 else 0) #1 là bookmarked, NaN là không bookmarked
    return interactions_df