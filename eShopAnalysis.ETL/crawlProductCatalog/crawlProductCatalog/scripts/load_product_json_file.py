from uuid import uuid4
import pymongo
import json
from pymongo import MongoClient, InsertOne

client = MongoClient("conn string")
db = client.get_database("ProductCatalogDb")
collection = db.get_collection("ProductCollection")
collection.delete_many({})

# will read every line of file and convert into a json string
# then convert json string into a dictionary and insert into mongodb
with open("absolute path to json file",
           mode="r", 
           encoding="utf-8") as f:
    for jsonObj in f:
        print(jsonObj)
        print(type(jsonObj))
        try:            
            itemDict = json.loads(jsonObj)        
        except UnicodeDecodeError as e:
            print(f"UnicodeDecodeError: {e}")
            continue
        except json.JSONDecodeError as e:
            print(f"JSONDecodeError: {e}")
            continue
        except Exception as e:
            print(f"An error occurred: {e}")
            continue
        
        uuidStr = uuid4().__str__()
        collection.insert_one(dict(itemDict, _id=uuidStr))
client.close()