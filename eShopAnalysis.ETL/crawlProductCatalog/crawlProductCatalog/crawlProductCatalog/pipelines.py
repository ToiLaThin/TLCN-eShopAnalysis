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

from crawlProductCatalog.items import CatalogItem, ProductItem

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

class ProcessCatalogPipeline(object):
    def __init__(self) -> None:
        settings = {
            "MONGODB_SERVER": "localhost",
            "MONGODB_PORT": 27017,
            "MONGODB_DB": "ProductCatalogDb",
            "MONGODB_CATALOG_COLLECTION": "CatalogCollection",
            "MONGODB_PRODUCT_COLLECTION": "ProductCollection"
        }        
    
    '''Private method called by process_item to process catalog item'''
    def __process_subcatalog(self, subcatalog_item):
        subcatalog_item['SubCatalogName'] = subcatalog_item['SubCatalogName'].replace("\\n", "").strip().upper()          

    def process_item(self, item, spider):        
        if isinstance(item, CatalogItem):
            for subcatalog_item in item['CatalogSubCatalogs']:
                self.__process_subcatalog(subcatalog_item)
        return item

class ProcessProductPipeline(object):
    def __init__(self) -> None:
        settings = {
            "MONGODB_SERVER": "localhost",
            "MONGODB_PORT": 27017,
            "MONGODB_DB": "ProductCatalogDb",
            "MONGODB_CATALOG_COLLECTION": "CatalogCollection",
            "MONGODB_PRODUCT_COLLECTION": "ProductCollection"
        }
        connection = pymongo.MongoClient(uuidRepresentation="standard", host=settings["MONGODB_SERVER"], port=settings["MONGODB_PORT"])
        db = connection[settings["MONGODB_DB"]]
        self.catalog_collection = db.get_collection(settings["MONGODB_CATALOG_COLLECTION"])

    def __normalize_product_subcatalog_name(self, name: str):
        result: str = name
        result = result.replace('\\n', '').strip().upper()
        return result

    def __process_product(self, product_item):
        # process product name and model
        if (str(product_item['ProductName']).find('(') == -1):
            return
        product_name, info = str(product_item['ProductName']).split('(')[0].strip(), str(product_item['ProductName']).split('(')[1].strip()
        print("Info", info) # info do not have (
        product_item['ProductName'] = product_name


        # process subcatalog name and id for product
        product_subcatalog_name = self.__normalize_product_subcatalog_name(product_item['SubCatalogName'])
        product_item['SubCatalogName'] = product_subcatalog_name
        catalog_subcatalog_cursor_dict = self.catalog_collection.find_one({"CatalogSubCatalogs.SubCatalogName": product_subcatalog_name})
        #find one only return the _id if have { "name": 1}, only _id, but this is id of the catalog, not the subcatalog
        if catalog_subcatalog_cursor_dict is None:
            raise DropItem(f"Can not find subcatalog name: {product_item['SubCatalogName']}")
        # print("***********HOW CURSOR LOOKS LIKE**************")
        # print(catalog_subcatalog_cursor_dict) # it return whole document with all properties
        # print(type(catalog_subcatalog_cursor_dict)) # a dict

        all_subcatalogs = catalog_subcatalog_cursor_dict['CatalogSubCatalogs']
        matching_subcatalog = [subcatalog for subcatalog in all_subcatalogs if subcatalog['SubCatalogName'] == product_subcatalog_name]
        #this is intent to be only single but since we use list comprehension, it will return a list, i cannot break
        product_item['SubCatalogId'] = matching_subcatalog[0]['_id']

    def process_item(self, item, spider):
        if isinstance(item, ProductItem):
            self.__process_product(item)
        return item