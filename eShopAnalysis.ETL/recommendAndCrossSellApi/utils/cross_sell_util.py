import pandas as pd
from mlxtend.preprocessing import TransactionEncoder
from mlxtend.frequent_patterns import apriori
from mlxtend.frequent_patterns import association_rules
from utils.share_util import get_root_path
import itertools
import os

def generate_and_save_association_rules():
    df = pd.read_csv(get_root_path() + '/resources/cart_item_transactions.csv')
    basket = df.groupby(['order_key']).agg({
        'product_key': lambda x: list(x),
        'product_name': lambda x: list(x),
        'subcatalog_name': lambda x: list(x),
    })
    basket.rename(
        columns={
            'product_key': 'product_keys', 
            'product_name': 'product_names', 
            'subcatalog_name': 'subcatalog_names'
        }, 
        inplace=True
    )
    te = TransactionEncoder()
    te_ary = te.fit(basket['product_keys'].values).transform(basket['product_keys'].values)
    df2 = pd.DataFrame(te_ary, columns=te.columns_) #te.columns_ is the unique item names
    frequent_itemsets = apriori(df2, min_support=0.02, use_colnames=True)

    # this took too long to run
    rules = association_rules(frequent_itemsets, metric="confidence", min_threshold=0.3)
    rules["antecedents"] = rules["antecedents"].apply(lambda x: ','.join(list(x)))
    rules["consequents"] = rules["consequents"].apply(lambda x: ','.join(list(x)))
    rules.to_csv(get_root_path() + '/resources/rules.csv', index=False)

def get_combinations_of_product_keys(keys: list[str]) -> list[tuple[str]]:
    all_combinations_of_all_len2 = []
    for len2 in range(1, len(keys) + 1):
        list_of_combination_of_len2 = list(itertools.combinations(keys, len2))
        all_combinations_of_all_len2.extend(list_of_combination_of_len2)
    return all_combinations_of_all_len2

def antecendents_set_match_combinations(antecedents_set: str, tuple_of_combinations_list: list[tuple[str]]) -> bool:
    # convert tuple to set
    for tuple_of_combination in tuple_of_combinations_list:
        set_combination = set(tuple_of_combination)
        # check if set_combination is equal of frozen_set
        if set_combination == antecedents_set:
            return True
    return False

def get_cross_selling_product_keys_of_combinations(product_keys_combinations: list[tuple]) -> list[str]:
    if not os.path.exists(get_root_path() + '/resources/rules.csv'):
        generate_and_save_association_rules()
    rules = pd.read_csv(get_root_path() + '/resources/rules.csv') # ghi hoặc đọc file sai rồi
    rules['antecedents'] = rules['antecedents'].apply(lambda x: set(x.split(',')))
    rules['consequents'] = rules['consequents'].apply(lambda x: set(x.split(',')))    

    set_consequents_list = rules[rules['antecedents'].apply(lambda antecedents: antecendents_set_match_combinations(antecedents, product_keys_combinations))]['consequents'].values    
    result = []
    avoid_duplicate_dict = {}
    for set_consequence in set_consequents_list:
        for product in set_consequence:
            if product in avoid_duplicate_dict:
                continue
            avoid_duplicate_dict[product] = 1
            result.append(product)
    return result