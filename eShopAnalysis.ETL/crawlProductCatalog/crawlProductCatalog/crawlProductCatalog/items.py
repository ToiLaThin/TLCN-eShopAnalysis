# Define here the models for your scraped items
#
# See documentation in:
# https://docs.scrapy.org/en/latest/topics/items.html

import scrapy


class CatalogItem(scrapy.Item):
    _id = scrapy.Field()
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

class ProductItem(scrapy.Item):
    _id = scrapy.Field()
    ProductName = scrapy.Field()
    SubCatalogId = scrapy.Field()
    SubCatalogName = scrapy.Field()
    ProductCoverImage = scrapy.Field()
    IsOnSale = scrapy.Field()
    ProductDisplaySaleValue = scrapy.Field()
    ProductDisplaySaleType = scrapy.Field()
    ProductDisplayPriceOnSale = scrapy.Field()
    HaveVariants = scrapy.Field()
    HavePricePerCublic = scrapy.Field()
    Revision = scrapy.Field()
    BusinessKey = scrapy.Field()
    # another Item
    ProductInfo = scrapy.Field()
    ProductModels = scrapy.Field()

class ProductInfoItem(scrapy.Item):
    ProductBrand = scrapy.Field()
    ProductDescription = scrapy.Field()
    ProductUsageInstruction = scrapy.Field()
    ProductPreserveInstruction = scrapy.Field()
    ProductIngredients = scrapy.Field()

class ProductModelItem(scrapy.Item):
    _id = scrapy.Field()
    ProductModelThumbnails = scrapy.Field()
    CublicType = scrapy.Field()
    PricePerCublicValue = scrapy.Field()
    CublicPrice = scrapy.Field()
    Price = scrapy.Field()
    SaleItemId = scrapy.Field()
    IsOnSaleModel = scrapy.Field()
    SaleValueModel = scrapy.Field()
    SaleType = scrapy.Field()
    PriceOnSaleModel = scrapy.Field()