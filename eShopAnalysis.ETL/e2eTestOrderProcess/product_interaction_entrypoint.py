
import json
from pymongo import MongoClient
from flows import product_interaction_flow

conn_string = "mongodb://localhost:27017/"
db_name = "ProductInteractionDb"
collection_names = ["LikeCollection", "RateCollection", "BookmarkCollection"]
#clear interaction db
client = MongoClient(conn_string)
db = client[db_name]
for collection_name in collection_names:
    db.get_collection(collection_name).delete_many({})


f = open('user-frequent.json', 'r')
users_frequent = json.load(f)
for user in users_frequent:
    user_name = user['username']
    password = user['password']
    profiles = user['profiles']
    
    # pick one or many profile by random
    purposes = [profile['purpose'] for profile in profiles]
    items = []
    for profile in profiles:
        items += profile['items']
    print(purposes, end='\n\n')
    print(items, sep='\n', end='\n********************************\n\n')
    
    product_interaction_flow(user_name, password, items)



