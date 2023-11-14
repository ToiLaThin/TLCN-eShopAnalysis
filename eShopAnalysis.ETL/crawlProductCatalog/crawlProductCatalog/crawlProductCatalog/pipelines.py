# Define your item pipelines here
#
# Don't forget to add your pipeline to the ITEM_PIPELINES setting
# See: https://docs.scrapy.org/en/latest/topics/item-pipeline.html


# useful for handling different item types with a single interface
from itemadapter import ItemAdapter
from scrapy.exceptions import DropItem
import pymongo
from bson.binary import UuidRepresentation
from uuid import uuid4

class SaveToMongoPipeline(object):
    def __init__(self):
        settings = {
            "MONGODB_SERVER": "localhost",
            "MONGODB_PORT": 27017,
            "MONGODB_DB": "ProductCatalogDb",
            "MONGODB_CATALOG_COLLECTION": "CatalogCollection",
            "MONGODB_PRODUCT_COLLECTION": "ProductCollection"
        }
        connection = pymongo.MongoClient(uuidRepresentation="standard", host=settings["MONGODB_SERVER"], port=settings["MONGODB_PORT"])

        db = connection[settings["MONGODB_DB"]]
        # when running different spider, remember to change the collection name
        self.collection = db.get_collection(settings["MONGODB_PRODUCT_COLLECTION"])
        self.collection.delete_many({}) # clear collection

    def process_item(self, item, spider):
        valid = True
        for data in item:
            if not data:
                valid = False
                raise DropItem(f"Missing {data}!")
        if valid:
            #https://pymongo.readthedocs.io/en/stable/examples/uuid.html pymongo support uuid
            uuid_obj = uuid4().__str__()
            print(f"uuid: {uuid_obj}")
            self.collection.insert_one(dict(item, _id=uuid_obj))
            print("Insert data successfully!")
        return item
