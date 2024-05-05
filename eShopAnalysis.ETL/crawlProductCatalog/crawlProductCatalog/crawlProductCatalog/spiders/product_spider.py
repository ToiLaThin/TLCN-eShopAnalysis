import json
import time
from uuid import uuid4
import pymongo
import scrapy
from slugify import slugify
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from crawlProductCatalog.items import CrawlItem, ProductInfoItem, ProductItem, ProductModelItem

class ProductspiderSpider(scrapy.Spider):
    name = "product_spider"
    allowed_domains = ["shop.annam-gourmet.com"]
    start_urls = ["https://shop.annam-gourmet.com/"]

    def __init__(self, **kwargs):
        super().__init__(**kwargs)
        self.crawl_items = self.get_crawl_data()
        self.dict_crawl_items = self.get_crawl_dict(self.crawl_items)   
        self.driver = webdriver.Chrome()
        self.driver.implicitly_wait(10)
        self.wait = WebDriverWait(self.driver, 10)

    def get_crawl_data(self) -> list[CrawlItem]:
        crawl_items: list[CrawlItem] = []
        f = open('./crawlProductCatalog/crawl.json', 'r') #path from the root folder
        crawl_datas = json.load(f)
        f.close()
        for crawl_data in crawl_datas:
            crawl_item = CrawlItem(crawl_data["catalog"], crawl_data["subcatalogs_products"])
            crawl_items.append(crawl_item)
        return crawl_items
    
    def get_crawl_dict(self, crawl_items: list[CrawlItem]) -> dict[str, CrawlItem]:
        dict_crawl_items = {crawl_item.catalog: crawl_item for crawl_item in crawl_items }
        return dict_crawl_items


    def parse(self, response):
        all_catalog_cols = response.css('.categories-menu .sm_megamenu_title .sm_megamenu_title')

        for catalog_col in all_catalog_cols:
            catalog_name = catalog_col.css('a.sm_megamenu_nodrop span.sm_megamenu_title_lv-2::text').get()
            catalog_slug = slugify(catalog_name)

            if catalog_slug == 'pet-care' or catalog_slug == 'fresh-food':
                continue
            if catalog_slug in self.dict_crawl_items:
                catalog_detail_url = catalog_col.css('a.sm_megamenu_nodrop::attr(href)').get()
                yield response.follow(catalog_detail_url,callback=self.parse_catalog_detail, meta={
                    "crawl_item": self.dict_crawl_items[catalog_slug], 
                    "catalog_name": catalog_name
                })

    def parse_catalog_detail(self, response):
        # this is the correct way to get the meta data
        crawl_item: CrawlItem = response.meta['crawl_item']
        catalog_name = response.meta['catalog_name']


        catalog_slug = crawl_item.catalog #or slugify(catalog_name)
        subcatalogs_products = crawl_item.subcatalogs_products
        subcatalog_slugs = [sub_and_products.subcatalog_name for sub_and_products in subcatalogs_products]
        all_subcatalogs = response.css('.filter-options-content .items .item')
        for subcatalog in all_subcatalogs:
            subcatalog_name = subcatalog.css('a::text').get()            
            subcatalog_detail_url = subcatalog.css('a::attr(href)').get()
            subcatalog_slug = slugify(subcatalog_name)            
            if subcatalog_slug not in subcatalog_slugs:
                continue
            product_names = subcatalogs_products[subcatalog_slugs.index(subcatalog_slug)].product_names
            yield response.follow(subcatalog_detail_url, callback=self.parse_subcatalog_detail, meta={
                "catalog_name": catalog_name,
                "subcatalog_name": subcatalog_name,
                "product_names": product_names
            })

    def parse_subcatalog_detail(self, response):
        catalog_name = response.meta['catalog_name']
        subcatalog_name = response.meta['subcatalog_name']
        product_names = list(response.meta['product_names']) # convert tuple to list
        normalized_subcatalog_name = subcatalog_name.replace('\\n', '').strip().upper()
        
        # # get subcatalog id # this is not needed anymore because it is handled in the pipeline
        # settings = {
        #     "CONNECTION_STRING": "mongodb://localhost:27017",
        #     "MONGODB_DB": "ProductCatalogDb",
        #     "MONGODB_CATALOG_COLLECTION": "CatalogCollection",
        #     "MONGODB_PRODUCT_COLLECTION": "ProductCollection"
        # }        
        # connection = pymongo.MongoClient(settings["CONNECTION_STRING"])
        # db = connection[settings["MONGODB_DB"]]
        # self.catalog_collection = db.get_collection(settings["MONGODB_CATALOG_COLLECTION"])
        # normalized_subcatalog_name = subcatalog_name.replace('\\n', '').strip().upper()
        # catalog_subcatalog_cursor_dict = self.catalog_collection.find_one({"CatalogSubCatalogs.SubCatalogName": normalized_subcatalog_name})
        # if catalog_subcatalog_cursor_dict is None:
        #     print("***********ERR**************")
        #     return
        # for catalog_subcatalog in catalog_subcatalog_cursor_dict["CatalogSubCatalogs"]:
        #     if catalog_subcatalog["SubCatalogName"] == normalized_subcatalog_name:
        #         subcatalog_id = catalog_subcatalog["_id"]
        #         break
        
        all_products = response.css('.hungvotan .products .product-item')
        next_page = response.css('.pages .pages-items .pages-item-next')
        for product in all_products:
            product_name = product.css('.product-item-info .product-item-details .product-item-name a::text').get()
            product_name = product_name.strip()
            if product_name not in product_names:
                continue
            product_names.remove(product_name)
            product_detail_url = product.css('.product-item-info .product-item-details .product-item-name a::attr(href)').get()
            yield response.follow(product_detail_url, callback=self.parse_product_detail, meta={
                "catalog_name": catalog_name,
                "subcatalog_name": normalized_subcatalog_name,
                "product_name": product_name,
                # "subcatalog_id": subcatalog_id #subcatalog_id is handled in pipeline
            })

        # if there product names to be crawled is still left and there is a next page
        # then crawl the next page with this same logic
        if len(product_names) > 0 and next_page:
            next_page_url = next_page.css('a::attr(href)').get()
            yield response.follow(next_page_url, callback=self.parse_subcatalog_detail, meta={
                "catalog_name": catalog_name,
                "subcatalog_name": subcatalog_name,
                "product_names": product_names
            })
        

    def parse_product_detail(self, response):
        catalog_name = response.meta['catalog_name']
        product_name = response.meta['product_name']
        subcatalog_name = response.meta['subcatalog_name']
        # subcatalog_id = response.meta['subcatalog_id'] #subcatalog_id is handled in pipeline

        self.driver.get(response.url)
        self.wait.until(EC.presence_of_element_located((By.CSS_SELECTOR, '.gallery-placeholder .fotorama-item .fotorama__wrap .fotorama__stage .fotorama__stage__shaft .fotorama__stage__frame img')))
        time.sleep(.5)
        product_cover_image = self.driver.find_element(
            By.CSS_SELECTOR, 
            '.gallery-placeholder .fotorama-item .fotorama__wrap .fotorama__stage .fotorama__stage__shaft .fotorama__stage__frame img'
        ).get_attribute('src')        

        self.wait.until(EC.presence_of_element_located((By.CSS_SELECTOR, '.product-info-price .product-info-brand .brand .value')))
        self.wait.until(EC.presence_of_element_located((By.CSS_SELECTOR, '.price-box .price-container span.price')))
        product_brand = self.driver.find_element(
            By.CSS_SELECTOR, 
            '.product-info-price .product-info-brand .brand .value'
        ).text
        product_model_price = self.driver.find_element(
            By.CSS_SELECTOR, 
            '.price-box .price-container span.price'
        ).text

        content_section_exist = True
        try:
            self.wait.until(EC.presence_of_all_elements_located((By.CSS_SELECTOR, '.content .product')))
        except Exception as e:
            print(f"Error in wait, the content section might not exist: {e}")
            content_section_exist = False

        if not content_section_exist:
            product_description = 'None'
            product_ingredients = 'None'
            product_usage_instruction = 'None'
            product_preserve_instruction = 'None'
        else:
            # because the content section exist, but not all 4 of them exist, so we need to handle each of them
            try:
                product_description = self.driver.find_element(
                    By.CSS_SELECTOR, 
                    '.content .product.description .value'
                ).text
            except Exception as e:
                product_description = 'None'

            try:
                product_ingredients = self.driver.find_element(
                    By.CSS_SELECTOR, 
                    '.content .product.ingredients .value'
                ).text
            except Exception as e:
                product_ingredients = 'None'

            try:
                product_usage_instruction = self.driver.find_element(
                    By.CSS_SELECTOR, 
                    '.content .product.instruction_for_use .value'
                ).text
            except Exception as e:
                product_usage_instruction = 'None'
            
            
            try:
                product_preserve_instruction = self.driver.find_element(
                    By.CSS_SELECTOR, 
                    '.content .product.storage_instructions .value'
                ).text
            except Exception as e:
                product_preserve_instruction = 'None'

        product_info_item = ProductInfoItem()
        product_info_item['ProductBrand'] = product_brand
        product_info_item['ProductDescription'] = product_description
        product_info_item['ProductUsageInstruction'] = product_usage_instruction
        product_info_item['ProductPreserveInstruction'] = product_preserve_instruction
        product_info_item['ProductIngredients'] = product_ingredients

        product_model_item = ProductModelItem()
        product_model_item['_id'] = uuid4().__str__()
        product_model_item['ProductModelThumbnails'] = []
        product_model_item['CublicType'] = None
        product_model_item['CublicValue'] = None
        product_model_item['PricePerCublicValue'] = None
        product_model_item['CublicPrice'] = None
        product_model_item['Price'] = product_model_price if product_model_price else 0
        # these will be set in the pipelines

        product_model_item['SaleItemId'] = None
        product_model_item['IsOnSaleModel'] = False
        product_model_item['SaleValueModel'] = None
        product_model_item['SaleType'] = None
        product_model_item['PriceOnSaleModel'] = None
        
        product_item = ProductItem()
        product_item['_id'] = uuid4().__str__()
        product_item['ProductName'] = product_name
        product_item['SubCatalogId'] = None #subcatalog_id is handled in pipeline
        product_item['SubCatalogName'] = subcatalog_name
        product_item['ProductCoverImage'] = product_cover_image
        product_item['IsOnSale'] = False
        product_item['ProductDisplaySaleValue'] = None
        product_item['ProductDisplaySaleType'] = None
        product_item['ProductDisplayPriceOnSale'] = None
        product_item['HaveVariants'] = False
        product_item['HavePricePerCublic'] = False
        product_item['Revision'] = 0
        product_item['BusinessKey'] = uuid4().__str__()
        product_item['ProductInfo'] = product_info_item
        product_item['ProductModels'] = [product_model_item]
        yield product_item