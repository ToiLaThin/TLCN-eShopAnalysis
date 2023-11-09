# Define here the models for your scraped items
#
# See documentation in:
# https://docs.scrapy.org/en/latest/topics/items.html

import scrapy


class CatalogItem(scrapy.Item):
    CatalogName = scrapy.Field()
    CatalogDescription = scrapy.Field()
    CatalogSubCatalogs = scrapy.Field()
    # CatalogSubCatalogs is list of SubCatalogItem
    # https://stackoverflow.com/questions/11184557/how-to-implement-nested-item-in-scrapy 
    # this is how to store nested item list

class SubCatalogItem(scrapy.Item):
    _id = scrapy.Field()
    SubCatalogName = scrapy.Field()
    SubCatalogDescription = scrapy.Field()
    SubCatalogImage = scrapy.Field()
