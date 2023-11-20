# Scrapy settings for crawlProductCatalog project
#
# For simplicity, this file contains only settings considered important or
# commonly used. You can find more settings consulting the documentation:
#
#     https://docs.scrapy.org/en/latest/topics/settings.html
#     https://docs.scrapy.org/en/latest/topics/downloader-middleware.html
#     https://docs.scrapy.org/en/latest/topics/spider-middleware.html

from shutil import which


BOT_NAME = "crawlProductCatalog"
#limit the number of items to be scraped
CLOSESPIDER_ITEMCOUNT = 2
#xtremely necessary for debugging
LOG_LEVEL='INFO' 

SPIDER_MODULES = ["crawlProductCatalog.spiders"]
NEWSPIDER_MODULE = "crawlProductCatalog.spiders"
FEEDS = {
    # 'catalogs.json': {'format': 'json'},
    # 'catalogs.csv': {'format': 'csv'},
    # 'products.json': {'format': 'json'},
}
SCRAPEOPS_API_KEY = 'do not reveal your key'
SCRAPEOPS_FAKE_USER_AGENT_ENDPOINT = 'https://headers.scrapeops.io/v1/user-agents?'

SCRAPEOPS_FAKE_USER_AGENT_ENDPOINT = 'https://headers.scrapeops.io/v1/user-agents?'
SCRAPEOPS_FAKE_USER_AGENT_ENABLED = True,
SCRAPEOPS_NUM_RESULTS = 50

MONGODB_SERVER = "localhost",
MONGODB_PORT = 27017,
MONGODB_DB = "ProductCatalogDb",
MONGODB_CATALOG_COLLECTION = "CatalogCollection",
# Crawl responsibly by identifying yourself (and your website) on the user-agent
#USER_AGENT = "crawlProductCatalog (+http://www.yourdomain.com)"

# For Selenium Scrapy
# https://stackoverflow.com/questions/8550114/can-scrapy-be-used-to-scrape-dynamic-content-from-websites-that-are-using-ajax
# https://copyprogramming.com/howto/python-scrapy-selenium-wait-until-code-example
# ONLY IF YOU USE SCRAPY SELENIUM WHICH IS NOT FLEXIBLE ENOUGH
# SELENIUM_DRIVER_NAME = 'chrome'
# SELENIUM_DRIVER_EXECUTABLE_PATH = which('chromedriver')
# SELENIUM_DRIVER_ARGUMENTS=['--headless'] 
# this to not launch a new browser
# '--headless' if using chrome instead of firefox

# Obey robots.txt rules
ROBOTSTXT_OBEY = False

# Configure maximum concurrent requests performed by Scrapy (default: 16)
#CONCURRENT_REQUESTS = 32

# Configure a delay for requests for the same website (default: 0)
# See https://docs.scrapy.org/en/latest/topics/settings.html#download-delay
# See also autothrottle settings and docs
#DOWNLOAD_DELAY = 3
# The download delay setting will honor only one of:
#CONCURRENT_REQUESTS_PER_DOMAIN = 16
#CONCURRENT_REQUESTS_PER_IP = 16

# Disable cookies (enabled by default)
#COOKIES_ENABLED = False

# Disable Telnet Console (enabled by default)
#TELNETCONSOLE_ENABLED = False

# Override the default request headers:
#DEFAULT_REQUEST_HEADERS = {
#    "Accept": "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8",
#    "Accept-Language": "en",
#}

# Enable or disable spider middlewares
# See https://docs.scrapy.org/en/latest/topics/spider-middleware.html
# SPIDER_MIDDLEWARES = {    
#    "crawlProductCatalog.middlewares.CrawlproductcatalogSpiderMiddleware": 543,
# }

# Enable or disable downloader middlewares
# See https://docs.scrapy.org/en/latest/topics/downloader-middleware.html
DOWNLOADER_MIDDLEWARES = {
    "crawlProductCatalog.middlewares.ScrapeOpsFakeBrowserHeaderAgentMiddleware": 200,
    # "scrapy_selenium.SeleniumMiddleware": 800
#    "crawlProductCatalog.middlewares.CrawlproductcatalogDownloaderMiddleware": 543,
}

# Enable or disable extensions
# See https://docs.scrapy.org/en/latest/topics/extensions.html
#EXTENSIONS = {
#    "scrapy.extensions.telnet.TelnetConsole": None,
#}

# Configure item pipelines
# See https://docs.scrapy.org/en/latest/topics/item-pipeline.html
ITEM_PIPELINES = {
#    "crawlProductCatalog.pipelines.CrawlproductcatalogPipeline": 300,
    "crawlProductCatalog.pipelines.ProcessCatalogPipeline": 100,
    "crawlProductCatalog.pipelines.ProcessProductPipeline": 150,
    "crawlProductCatalog.pipelines.SaveToMongoPipeline": 200,
}

# Enable and configure the AutoThrottle extension (disabled by default)
# See https://docs.scrapy.org/en/latest/topics/autothrottle.html
#AUTOTHROTTLE_ENABLED = True
# The initial download delay
#AUTOTHROTTLE_START_DELAY = 5
# The maximum download delay to be set in case of high latencies
#AUTOTHROTTLE_MAX_DELAY = 60
# The average number of requests Scrapy should be sending in parallel to
# each remote server
#AUTOTHROTTLE_TARGET_CONCURRENCY = 1.0
# Enable showing throttling stats for every response received:
#AUTOTHROTTLE_DEBUG = False

# Enable and configure HTTP caching (disabled by default)
# See https://docs.scrapy.org/en/latest/topics/downloader-middleware.html#httpcache-middleware-settings
#HTTPCACHE_ENABLED = True
#HTTPCACHE_EXPIRATION_SECS = 0
#HTTPCACHE_DIR = "httpcache"
#HTTPCACHE_IGNORE_HTTP_CODES = []
#HTTPCACHE_STORAGE = "scrapy.extensions.httpcache.FilesystemCacheStorage"

# Set settings whose default value is deprecated to a future-proof value
REQUEST_FINGERPRINTER_IMPLEMENTATION = "2.7"
TWISTED_REACTOR = "twisted.internet.asyncioreactor.AsyncioSelectorReactor"
FEED_EXPORT_ENCODING = "utf-8"
