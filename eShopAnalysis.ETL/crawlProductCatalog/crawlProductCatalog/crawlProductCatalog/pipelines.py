# Define your item pipelines here
#
# Don't forget to add your pipeline to the ITEM_PIPELINES setting
# See: https://docs.scrapy.org/en/latest/topics/item-pipeline.html


# useful for handling different item types with a single interface
import re
from itemadapter import ItemAdapter
from scrapy.exceptions import DropItem
import pymongo
from bson.binary import UuidRepresentation
from uuid import uuid4

from crawlProductCatalog.items import CatalogItem, ProductItem, CublicType

class SaveToMongoPipeline(object):
    def __init__(self):
        settings = {
            "CONNECTION_STRING": "your connection string",
            "MONGODB_DB": "ProductCatalogDb",
            "MONGODB_CATALOG_COLLECTION": "CatalogCollection",
            "MONGODB_PRODUCT_COLLECTION": "ProductCollection"
        }
        connection = pymongo.MongoClient(settings["CONNECTION_STRING"])

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
            "CONNECTION_STRING": "your connection string",
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
            "CONNECTION_STRING": "your connection string",
            "MONGODB_DB": "ProductCatalogDb",
            "MONGODB_CATALOG_COLLECTION": "CatalogCollection",
            "MONGODB_PRODUCT_COLLECTION": "ProductCollection"
        }
        connection = pymongo.MongoClient(settings["CONNECTION_STRING"])
        db = connection[settings["MONGODB_DB"]]
        self.catalog_collection = db.get_collection(settings["MONGODB_CATALOG_COLLECTION"])

    def __normalize_product_subcatalog_name(self, name: str):
        result: str = name
        result = result.replace('\\n', '').strip().upper()
        return result

    def __process_product(self, product_item):
        # process product price
        product_price = product_item['ProductModels'][0]['Price']
        # print("Product price", product_price)
        product_price = float(re.sub(r"\D", "", product_price))
        # print("Product price after changed", product_price)
        # print("Product price after changed", type(product_price))
        product_item['ProductModels'][0]['Price'] = product_price


        
        # process subcatalog name and id for product
        product_subcatalog_name = self.__normalize_product_subcatalog_name(product_item['SubCatalogName'])
        product_item['SubCatalogName'] = product_subcatalog_name
        catalog_subcatalog_cursor_dict = self.catalog_collection.find_one({"CatalogSubCatalogs.SubCatalogName": product_subcatalog_name})
        #find one only return the _id if have { "name": 1}, only _id, but this is id of the catalog, not the subcatalog
        if catalog_subcatalog_cursor_dict is None:
            print("***********ERR**************")
            print(f"Can not find subcatalog name: {product_item['SubCatalogName']} of product {product_item['ProductName']}, will drop")
            raise DropItem()
        # print("***********HOW CURSOR LOOKS LIKE**************")
        # print(catalog_subcatalog_cursor_dict) # it return whole document with all properties
        # print(type(catalog_subcatalog_cursor_dict)) # a dict

        all_subcatalogs = catalog_subcatalog_cursor_dict['CatalogSubCatalogs']
        matching_subcatalog = [subcatalog for subcatalog in all_subcatalogs if subcatalog['SubCatalogName'] == product_subcatalog_name]
        #this is intent to be only single but since we use list comprehension, it will return a list, i cannot break
        if len(matching_subcatalog) == 0:
            print("***********ERR**************")
            print(f"No matching subcatalog name to get sub catalog id: {product_item['SubCatalogName']} of product {product_item['ProductName']}, will drop")
            raise DropItem()
        product_item['SubCatalogId'] = matching_subcatalog[0]['_id']


        # process product name and model
        product_item_name = str(product_item['ProductName'])
        if (product_item_name.find('(') == -1):
            print("*************INFO*****************")
            print("Product name do not have (, will return and still save to db")
            # TODO if product name do not have (, it can still have cublic value and type, so we still need to check on that
            return
        
        product_name, info = product_item_name.split('(')[0].strip(), product_item_name.split('(')[1].strip()
        print("Info", info) # info do not have (
        # product_item['ProductName'] = product_name #ko cần bỏ đại lượng, vì nó ảnh hưởng tới việc find product theo tên từ file association
        try:
            # sometimes it's (g), so no match
            match_cublic_value = re.match(r"\d+",info)
            match_cublic_type = re.search(r"[a-zA-Z]+", info)
            cublic_value = int(match_cublic_value.group())
            cublic_type_str = match_cublic_type.group()
        except Exception as e: #can be caused by (g), no int inside bracket
            print("*************ERROR*****************")
            print("Error in processing the cublic value and type")
            cublic_value = 100 # assign random value, not DropItem because it can still have cublic type like (g)
            cublic_type_str = "g"
            # raise DropItem()
        product_item['ProductModels'][0]['CublicValue'] = cublic_value
        lowered_cublic_type_str = cublic_type_str.lower()
        if lowered_cublic_type_str == "ml" or lowered_cublic_type_str == "l":
            product_item['ProductModels'][0]['CublicType'] = CublicType.ML.value
        elif lowered_cublic_type_str == "mg" or lowered_cublic_type_str == "g" or lowered_cublic_type_str == "kg" or lowered_cublic_type_str == "gr":
                product_item['ProductModels'][0]['CublicType'] = CublicType.MG.value
        elif lowered_cublic_type_str == "m" or lowered_cublic_type_str == "cm" or lowered_cublic_type_str == "dm":
                product_item['ProductModels'][0]['CublicType'] = CublicType.M.value
        else:
                product_item['ProductModels'][0]['CublicType'] = CublicType.NONE.value            
        # print(product_item['ProductModels'][0]['CublicValue'])
        # print(type(product_item['ProductModels'][0]['CublicValue']))
        # print(product_item['ProductModels'][0]['CublicType'])
        # print(type(product_item['ProductModels'][0]['CublicType']))

       

    def process_item(self, item, spider):
        if isinstance(item, ProductItem):
            try:
                self.__process_product(item)
            except Exception as e:
                print("*************ERROR*****************")
                print("Error in processing the item")
                print(item)
                return
        return item