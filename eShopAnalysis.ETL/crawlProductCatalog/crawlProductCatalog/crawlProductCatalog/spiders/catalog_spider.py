import json
from uuid import uuid4
from crawlProductCatalog.items import CatalogItem, CrawlItem, SubCatalogItem
import scrapy
from slugify import slugify

class CatalogSpider(scrapy.Spider):
    name = "catalog_spider"
    allowed_domains = ["shop.annam-gourmet.com"]
    start_urls = ["https://shop.annam-gourmet.com/"]

    def __init__(self, **kwargs):
        super().__init__(**kwargs)
        self.crawl_items = self.get_crawl_data()
        self.dict_crawl_items = self.get_crawl_dict(self.crawl_items)

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

            if catalog_slug in ['eggs', 'care-dog', 'food-dog', 'care-cat', 'food-cat']:
                catalog_detail_url = catalog_col.css('a.sm_megamenu_nodrop::attr(href)').get()
                # eggs, care-dog, food-dog, care-cat, food-cat is actually not the catalog but subcatalog
                yield response.follow(catalog_detail_url,callback=self.parse_catalog_detail, meta={
                    "crawl_item": self.dict_crawl_items['fresh-food'] if catalog_slug == 'eggs' else self.dict_crawl_items['pet-care'], 
                    "catalog_name": 'FRESH FOOD' if catalog_slug == 'eggs' else 'PET CARE'
                })
            if catalog_slug in self.dict_crawl_items:
                catalog_detail_url = catalog_col.css('a.sm_megamenu_nodrop::attr(href)').get()
                yield response.follow(catalog_detail_url,callback=self.parse_catalog_detail, meta={
                    "crawl_item": self.dict_crawl_items[catalog_slug], 
                    "catalog_name": catalog_name
                })

    def parse_catalog_detail(self, response):
        # this is the correct way to get the meta data
        crawl_item = response.meta['crawl_item']
        catalog_name = response.meta['catalog_name']


        catalog_slug = crawl_item.catalog #or slugify(catalog_name)
        #if catalog_slug in ['eggs', 'care-dog', 'food-dog', 'care-cat', 'food-cat']:, this is wrong, we get the existing _id, CatalogSubCatalogs, and add to that
        catalog_item = CatalogItem()
        catalog_item['CatalogName'] = catalog_name
        catalog_item['CatalogDescription'] = ''
        #it insert multiple instance of catalog pet-care, fresh-food (eggs), so delete duplicates manually
        if catalog_slug == 'pet-care':
            catalog_item['_id'] = uuid4().__str__()
            catalog_item['CatalogSubCatalogs'] = []
            subcatalog_care_dog = SubCatalogItem()
            subcatalog_care_dog['_id'] = uuid4().__str__()
            subcatalog_care_dog['SubCatalogName'] = 'CARE DOG'
            subcatalog_care_dog['SubCatalogDescription'] = ''
            subcatalog_care_dog['SubCatalogImage'] = ''
            catalog_item['CatalogSubCatalogs'].append(subcatalog_care_dog)
            subcatalog_food_dog = SubCatalogItem()
            subcatalog_food_dog['_id'] = uuid4().__str__()
            subcatalog_food_dog['SubCatalogName'] = 'FOOD DOG'
            subcatalog_food_dog['SubCatalogDescription'] = ''
            subcatalog_food_dog['SubCatalogImage'] = ''
            catalog_item['CatalogSubCatalogs'].append(subcatalog_food_dog)
            subcatalog_care_cat = SubCatalogItem()
            subcatalog_care_cat['_id'] = uuid4().__str__()
            subcatalog_care_cat['SubCatalogName'] = 'CARE CAT'
            subcatalog_care_cat['SubCatalogDescription'] = ''
            subcatalog_care_cat['SubCatalogImage'] = ''
            catalog_item['CatalogSubCatalogs'].append(subcatalog_care_cat)
            subcatalog_food_cat = SubCatalogItem()
            subcatalog_food_cat['_id'] = uuid4().__str__()
            subcatalog_food_cat['SubCatalogName'] = 'FOOD CAT'
            subcatalog_food_cat['SubCatalogDescription'] = ''
            subcatalog_food_cat['SubCatalogImage'] = ''
            catalog_item['CatalogSubCatalogs'].append(subcatalog_food_cat)
            yield catalog_item
        else:
            catalog_item['_id'] = uuid4().__str__()
            catalog_item['CatalogSubCatalogs'] = []
            subcatalogs_products = crawl_item.subcatalogs_products
            subcatalog_slugs = [sub_and_products.subcatalog_name for sub_and_products in subcatalogs_products]
            all_subcatalogs = response.css('.filter-options-content .items .item')
            if (catalog_name == 'PET CARE'):
                all_subcatalogs = ['care-dog', 'food-dog', 'care-cat', 'food-cat']
            if (catalog_name == 'FRESH FOOD'):
                all_subcatalogs = ['eggs']
            for subcatalog in all_subcatalogs:
                if catalog_name not in ['PET CARE', 'FRESH FOOD']:
                    subcatalog_name = subcatalog.css('a::text').get()
                else:
                    subcatalog_name = subcatalog
                subcatalog_slug = slugify(subcatalog_name)            
                print(f'Catalog: {catalog_slug}, Subcatalog: {subcatalog_name}, Slug: {subcatalog_slug}')
                if subcatalog_slug not in subcatalog_slugs:
                    continue
                
                subcatalog_item = SubCatalogItem()
                subcatalog_item['_id'] = uuid4().__str__()
                subcatalog_item['SubCatalogName'] = subcatalog_name
                subcatalog_item['SubCatalogDescription'] = ''
                subcatalog_item['SubCatalogImage'] = ''
                catalog_item['CatalogSubCatalogs'].append(subcatalog_item)
            yield catalog_item