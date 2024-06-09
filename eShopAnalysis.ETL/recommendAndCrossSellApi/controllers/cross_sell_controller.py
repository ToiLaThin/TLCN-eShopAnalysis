from flask import request
from utils.cross_sell_util import get_combinations_of_product_keys, get_cross_selling_product_keys_of_combinations

def cross_sell():
    product_keys = request.json['product_keys']
    all_combinations = get_combinations_of_product_keys(keys=product_keys)
    result = get_cross_selling_product_keys_of_combinations(all_combinations)
    print("Result:", result)
    return result
