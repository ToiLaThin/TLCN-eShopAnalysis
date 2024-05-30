from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.common.action_chains import ActionChains
from selenium.webdriver.common.actions import action_builder
from time import sleep
from enum import Enum

class ItemQuantity(Enum):
    LOW = 'Low'
    MEDIUM = 'Medium'
    HIGH = 'High' 

#old version
class ItemProfile:
    def __init__(self, name: str, quantity: ItemQuantity, weight: float):
        self.name = name
        self.quantity = quantity
        self.weight = weight

class UserSession:
    def __init__(self, username: str, password: str, item_profiles: list[ItemProfile]):
        self.username = username
        self.password = password
        self.item_profiles = item_profiles

class Item:
    def __init__(self, name: str, subcatalog: str, quantity: ItemQuantity, weight: float):
        self.name = name
        self.subcatalog = subcatalog
        self.quantity = quantity
        self.weight = weight

class Profile:
    def __init__(self, purpose: str, weight: float, items: list[Item]):
        self.purpose = purpose
        self.weight = weight
        self.items = items

class UserSession:
    def __init__(self, username: str, password: str, profiles: list[Profile]):
        self.username = username
        self.password = password
        self.profiles = profiles

class Association:
    def __init__(self, related_subcatalogs: list[str], products: list[object]):
        self.related_subcatalogs = related_subcatalogs
        self.products = products

      
# https://stackoverflow.com/questions/52207164/python-selenium-highlight-element-doesnt-do-anything
def highlight(element, effect_time, color, border):
    """Highlights (blinks) a Selenium Webdriver element"""
    driver = element._parent
    def apply_style(s):
        driver.execute_script("arguments[0].setAttribute('style', arguments[1]);",
                              element, s)
    original_style = element.get_attribute('style')
    apply_style("border: {0}px solid {1};".format(border, color))
    sleep(effect_time)
    apply_style(original_style)
    
def order_confirm_flow(username_val, password_val, items: list[Item]):
    # Create a new instance of the Chrome
    options = webdriver.ChromeOptions()
    options.add_experimental_option("detach", True) #https://stackoverflow.com/questions/72261548/how-to-keep-browser-session-alive-for-10-min-on-heroku-using-selenium
    driver = webdriver.Chrome(options=options)
    driver.maximize_window()
    driver.implicitly_wait(10)# do not mix implicit and explicit waits, the time could increase
    # Navigate to the localhost:4200
    wait = WebDriverWait(driver, 20)
    driver.get("http://localhost:4200")

    def login_action():
        # wait for 5 seconds
        # find the login button and click it
        login_btn = driver.find_element(By.CLASS_NAME, 'login-btn')
        wait.until(EC.element_to_be_clickable((By.CLASS_NAME, 'login-btn')))
        wait.until(EC.invisibility_of_element((By.TAG_NAME, 'nz-notification')))
        login_btn.click()

        # find the username and password fields and fill them
        username = driver.find_element(By.ID, 'username')
        password = driver.find_element(By.ID, 'password')
        wait.until(EC.element_to_be_clickable((By.ID, 'username')))
        wait.until(EC.element_to_be_clickable((By.ID, 'password')))
        username.send_keys(username_val)
        password.send_keys(password_val)

        # find the login button and click it
        login_confirm = driver.find_element(By.CLASS_NAME, 'btn-success')
        wait.until(EC.element_to_be_clickable((By.CLASS_NAME, 'btn-success')))
        highlight(login_confirm, 2, 'red', 2)           
        login_confirm.click()
    login_action()

    from random import randint, choices, sample
    def decide_action() -> dict:
        # transform items to item to buy
        num_item_buy = randint(1, len(items))
        item_weights_buy = [item["weight"] for item in items]
        items_to_buy = choices(items, k=num_item_buy, weights=item_weights_buy)
        print("Before remove duplicate:", items_to_buy)

        #dict to remove duplicates, update the num_item_buy, so ui does not have to buy the same item multiple times
        dict_items = {}
        for item in items_to_buy:
            if item["name"] not in dict_items:
                dict_items[item["name"]] = item

        associations = []
        import glob
        import json
        for file in glob.glob("associations_*.json"):
            with open(file) as f:
                associations_in_a_file = json.load(f)
                associations = associations + associations_in_a_file

        #find all associations that have the related subcatalogs
        item_subcatalogs = [item["subcatalog"] for item in dict_items.values()]
        #items in association that user will buy will not be used again to buy from another associations
        for association in associations:
            if set(association["related_subcatalogs"]).intersection(item_subcatalogs):
                # print(association)
                selected_association_all_products = association["products"]
                selected_association_products_weights = [product["weight"] for product in selected_association_all_products]
                selected_association_products_names = [product["name"] for product in selected_association_all_products]
                if num_item_buy == 1 or num_item_buy == 2:
                    buy_from_association = choices([True, False], weights=[0.6, 0.4], k = 1)[0]
                else:
                    buy_from_association = choices([True, False], weights=[0.4, 0.6], k = 1)[0]
                print(f"User {username_val} will buy from association: {buy_from_association}")
                if buy_from_association == False:
                    continue
                buy_much = choices([True, False], weights=[0.2, 0.8], k = 1)[0]
                print(f"User {username_val} will buy much: {buy_much}")
                if buy_much == False:                    
                    num_item_buy_from_assocation = randint(1, 2) if len(selected_association_products_names) >= 2 else 1
                else:
                    from math import ceil
                    if len(selected_association_products_names) > 2:
                        maxrand =  ceil(len(selected_association_products_names) / 2) 
                        num_item_buy_from_assocation = randint(2, maxrand) 
                    else:
                        num_item_buy_from_assocation = randint(1, len(selected_association_products_names))
                association_product_names_will_buy = choices(selected_association_products_names, k=num_item_buy_from_assocation, weights=selected_association_products_weights)
                print(f"User {username_val} will buy {association_product_names_will_buy} because of association {association['related_subcatalogs']}")

                for product_name in association_product_names_will_buy:
                    if product_name not in dict_items:
                        dict_items[product_name] = {
                            "name": product_name,
                            "quantity": 'Low',
                        }
        return dict_items
    dict_items = decide_action()

    #unallocate the dict_items from memory
    items_to_buy = list(dict_items.values())
    num_item_buy = len(items_to_buy)
    print("After remove duplicate:", items_to_buy)
    del dict_items

    # find the search btn, search input
    # input the search value and click the search btn
    print(f"User {username_val} is buying {num_item_buy} items")
    for item_qty in items_to_buy:
      qty_str = item_qty["quantity"]
      print(f"Item: {item_qty['name']} Quantity: {qty_str}")
      if qty_str == 'High':
          qty_buy = choices([5, 6], weights=[0.5, 0.5])[0]
      elif qty_str == 'Medium':
          qty_buy = choices([3, 4], weights=[0.7, 0.3])[0]
      else:
          qty_buy = choices([1, 2], weights=[0.4, 0.3])[0]
      print(f"Buying {qty_buy} of {item_qty['name']}")

      wait.until(EC.invisibility_of_element((By.TAG_NAME, 'nz-notification')))
      search_input = driver.find_element(By.CLASS_NAME, 'global-search-input')
      search_input.clear()
      search_btn = driver.find_element(By.CLASS_NAME, 'global-search-confirm-btn')
      wait.until(EC.element_to_be_clickable((By.CLASS_NAME, 'global-search-input')))
      wait.until(EC.element_to_be_clickable((By.CLASS_NAME, 'global-search-confirm-btn')))
      item_name = item_qty["name"]
      search_input.send_keys(item_name)
      search_btn.click()
      sleep(2)
      try:
        product_card = driver.find_elements(By.CLASS_NAME, 'product-search-card')[0]
        product_searched_name = product_card.find_element(By.CLASS_NAME, 'product-search-card-title').text
        if item_name.lower() != product_searched_name.lower():
            print(f"Item {item_name} not found, could be not crawled successfully")
            continue
      except Exception as e:
        print(f"Item {item_name} not found, could be not crawled successfully")
        continue
      
      actions = ActionChains(driver).move_to_element(product_card).pause(1)
      actions.perform()
      view_btn = product_card.find_element(By.CLASS_NAME, 'view-search-btn')
      wait.until(EC.element_to_be_clickable(view_btn))
      view_btn.click()
      sleep(2)

      # find the quantity input and fill it, then click add to cart
      quantity_input = driver.find_element(By.CLASS_NAME, 'product-info-qty-input')
      add_to_cart_btn = driver.find_element(By.CLASS_NAME, 'product-info-add-to-cart-btn')
      wait.until(EC.element_to_be_clickable((By.CLASS_NAME, 'product-info-qty-input')))
      actions = ActionChains(driver).move_to_element(quantity_input).pause(1).click()
      actions.perform()
      for time in range(qty_buy - 1):
          quantity_input.send_keys(Keys.ARROW_UP)      
      add_to_cart_btn.click()
      

      close_quickview_btn = driver.find_element(By.CLASS_NAME, 'product-quick-view-close')
      wait.until(EC.element_to_be_clickable(close_quickview_btn))
      close_quickview_btn.click()

  
    # find the cart button and click it, close the search
    try:
        close_search_btn = driver.find_element(By.CLASS_NAME, 'global-search-close-btn')
        wait.until(EC.element_to_be_clickable(close_search_btn))
        close_search_btn.click()
    except Exception as e:
        print("Cannot close the search")


    def confirm_cart_and_order_action():
        cart_btn = driver.find_element(By.CLASS_NAME, 'cart-btn')
        wait.until(EC.element_to_be_clickable(cart_btn))
        cart_btn.click()


        cart_confirm_btn = driver.find_element(By.CLASS_NAME, 'cart-confirm-btn')
        wait.until(EC.element_to_be_clickable(cart_confirm_btn))
        cart_confirm_btn.click()


        # fill the address & phone form
        phone = '0123456789'
        address = '123 Nguyen Van Linh, Thu Duc, Ho Chi Minh City, Viet Nam'

        phone_input = driver.find_element(By.ID, 'customer-phone')
        address_input = driver.find_element(By.ID, 'customer-address')
        customer_info_confirm_btn = driver.find_element(By.CLASS_NAME, 'confirm-customer-info-btn')
        use_saved_location_btn = driver.find_element(By.CLASS_NAME, 'use-saved-location-btn')

        wait.until(EC.element_to_be_clickable(phone_input))
        wait.until(EC.element_to_be_clickable(address_input))    
        wait.until(EC.element_to_be_clickable(use_saved_location_btn))
        wait.until(EC.element_to_be_clickable(customer_info_confirm_btn))

        phone_input.send_keys(phone)
        address_input.send_keys(address)
        use_saved_location_btn.click()
        sleep(1)
        customer_info_confirm_btn.click()

        # choose the payment method, then confirm
        payment_method_cod_label = driver.find_element(By.CSS_SELECTOR, 'label[for="payment-method-0"]')
        wait.until(EC.element_to_be_clickable(payment_method_cod_label))
        payment_method_cod_label.click()
        sleep(2)
        confirm_payment_method_btn = driver.find_element(By.CLASS_NAME, 'confirm-payment-method-btn')
        wait.until(EC.element_to_be_clickable(confirm_payment_method_btn))
        confirm_payment_method_btn.click()        
        sleep(2)
    confirm_cart_and_order_action()

    def logout_action():
        logout_btn = driver.find_element(By.CLASS_NAME, 'logout-btn')
        wait.until(EC.invisibility_of_element((By.TAG_NAME, 'nz-notification')))
        wait.until(EC.element_to_be_clickable((By.CLASS_NAME, 'logout-btn')))
        highlight(logout_btn, 2, 'red', 2)
        logout_btn.click()
        wait.until(EC.url_to_be('http://localhost:4200/shopping/index'))
        print(driver.current_url)
        # wait for 5 seconds
        sleep(5)
    logout_action()
    # action = action_builder.ActionBuilder(driver)
    # action.pointer_action.move_to_location(200, 200).pause(3).move_to(login_btn).click()
    # action.perform()
            
    # close the browser window  
    driver.quit()

def product_interaction_flow(username_val, password_val, items: list[Item]):    
    options = webdriver.ChromeOptions()
    options.add_experimental_option("detach", True)
    driver = webdriver.Chrome(options=options)
    driver.maximize_window()
    driver.implicitly_wait(10)# do not mix implicit and explicit waits, the time could increase
    # Navigate to the localhost:4200
    wait = WebDriverWait(driver, 20)
    driver.get("http://localhost:4200")

    def login_action():
        # wait for 5 seconds
        # find the login button and click it
        login_btn = driver.find_element(By.CLASS_NAME, 'login-btn')
        wait.until(EC.element_to_be_clickable((By.CLASS_NAME, 'login-btn')))
        wait.until(EC.invisibility_of_element((By.TAG_NAME, 'nz-notification')))
        login_btn.click()

        # find the username and password fields and fill them
        username = driver.find_element(By.ID, 'username')
        password = driver.find_element(By.ID, 'password')
        wait.until(EC.element_to_be_clickable((By.ID, 'username')))
        wait.until(EC.element_to_be_clickable((By.ID, 'password')))
        username.send_keys(username_val)
        password.send_keys(password_val)

        # find the login button and click it
        login_confirm = driver.find_element(By.CLASS_NAME, 'btn-success')
        wait.until(EC.element_to_be_clickable((By.CLASS_NAME, 'btn-success')))
        login_confirm.click()
    login_action()

    from random import randint, choices, sample
    import glob
    import json
    import math
    def decide_action() -> dict:
        item_weights_interact = [item["weight"] for item in items]
        items_to_interact = items

        #dict to remove duplicates, update the num_item_buy, so ui does not have to buy the same item multiple times
        dict_items = {}
        for item in items_to_interact:
            if item["name"] not in dict_items:
                dict_items[item["name"]] = item

        associations = []
        for file in glob.glob("associations_*.json"):
            with open(file) as f:
                associations_in_a_file = json.load(f)
                associations = associations + associations_in_a_file

        #find all associations that have the related subcatalogs
        item_subcatalogs = [item["subcatalog"] for item in dict_items.values()]
        #items in association that user will buy will not be used again to buy from another associations
        for association in associations:
            if set(association["related_subcatalogs"]).intersection(item_subcatalogs):
                selected_association_all_products = association["products"]
                selected_association_products_weights = [product["weight"] for product in selected_association_all_products]
                selected_association_products_names = [product["name"] for product in selected_association_all_products]

                num_item_buy_from_assocation = randint(1, math.floor(len(selected_association_products_names) / 2))
                association_product_names_will_buy = choices(selected_association_products_names, k=num_item_buy_from_assocation, weights=selected_association_products_weights)
                # print(f"User {username_val} will buy {association_product_names_will_buy} because of association {association['related_subcatalogs']}")

                for product_name in association_product_names_will_buy:
                    if product_name not in dict_items:
                        dict_items[product_name] = {
                            "name": product_name,
                            "quantity": 'Low',
                        }
        return dict_items
    dict_items = decide_action()

    items_to_interact = list(dict_items.values())
    num_item_interact = len(items_to_interact)
    print("After remove duplicate:", items_to_interact)
    print(f"User {username_val} will interact with {num_item_interact} items")
    #unallocate the dict_items from memory
    del dict_items

    # find the search btn, search input
    # input the search value and click the search btn
    for item_qty in items_to_interact:
        wait.until(EC.invisibility_of_element((By.TAG_NAME, 'nz-notification')))
        search_input = driver.find_element(By.CLASS_NAME, 'global-search-input')
        search_input.clear()
        search_btn = driver.find_element(By.CLASS_NAME, 'global-search-confirm-btn')
        wait.until(EC.element_to_be_clickable((By.CLASS_NAME, 'global-search-input')))
        wait.until(EC.element_to_be_clickable((By.CLASS_NAME, 'global-search-confirm-btn')))
        item_name = item_qty["name"]
        search_input.send_keys(item_name)
        search_btn.click()
        sleep(2)
        try:
            product_card = driver.find_elements(By.CLASS_NAME, 'product-search-card')[0]
            product_searched_name = product_card.find_element(By.CLASS_NAME, 'product-search-card-title').text
            if item_name.lower() != product_searched_name.lower():
                print(f"Item {item_name} not found, could be not crawled successfully")
                continue
        except Exception as e:
            print(f"Item {item_name} not found, could be not crawled successfully")
            continue
        
        actions = ActionChains(driver).move_to_element(product_card).pause(1)
        actions.perform()
        view_btn = product_card.find_element(By.CLASS_NAME, 'view-search-btn')
        wait.until(EC.element_to_be_clickable(view_btn))
        view_btn.click()
        sleep(2)
        
        #< 0.2 weight => 3star
        #0.2 weight => 3.5star
        #0.25 weight => 4star
        #0.3 - 0.35 weight => 4.5star
        #> 0.35 weight => 5star
        try:
            item_weight = item_qty["weight"]
            if item_weight < 0.2:
                star = 3
            elif item_weight == 0.2:
                star = 3.5
            elif item_weight == 0.25:
                star = 4
            elif item_weight > 0.25 and item_weight <= 0.35:
                star = 4.5
            else:
                star = 5
        except Exception as e:
            star = choices([3, 3.5, 4, 4.5, 5], k=1, weights=[0.1, 0.25, 0.35, 0.2, 0.1])[0]      
        will_like = True if star >= 4 else False
        will_bookmark = True if star >= 4.5 else False
        # ko co weight => random star => tu star => liked, bookmarked
      
        # find the star rating and click it
        star_label = driver.find_element(By.CSS_SELECTOR, f'label[for="rating2-{star}"]')
        wait.until(EC.visibility_of(star_label))
        wait.until(EC.element_to_be_clickable(star_label))
        try:
            star_label.click()
        except Exception as e:
            print(f"Item {item_name} cannot be rated, could be not crawled successfully. Skip rating this item")
        sleep(1)

        # find the like button and click it
        if will_like:
            like_btn = driver.find_element(By.CLASS_NAME, 'product-info-like-interaction-btn')
            wait.until(EC.element_to_be_clickable(like_btn))
            like_btn.click()
            sleep(1)
        
        # find the bookmark button and click it
        if will_bookmark:
            bookmark_btn = driver.find_element(By.CLASS_NAME, 'product-info-bookmark-interaction-btn')
            wait.until(EC.element_to_be_clickable(bookmark_btn))
            bookmark_btn.click()
            sleep(1)

        close_quickview_btn = driver.find_element(By.CLASS_NAME, 'product-quick-view-close')
        wait.until(EC.element_to_be_clickable(close_quickview_btn))
        close_quickview_btn.click()

  
    # find the cart button and click it, close the search
    try:
        close_search_btn = driver.find_element(By.CLASS_NAME, 'global-search-close-btn')
        wait.until(EC.element_to_be_clickable(close_search_btn))
        close_search_btn.click()
    except Exception as e:
        print("Cannot close the search")

    def logout_action():
        logout_btn = driver.find_element(By.CLASS_NAME, 'logout-btn')
        wait.until(EC.invisibility_of_element((By.TAG_NAME, 'nz-notification')))
        wait.until(EC.element_to_be_clickable((By.CLASS_NAME, 'logout-btn')))
        logout_btn.click()
        wait.until(EC.url_to_be('http://localhost:4200/shopping/index'))
        print(driver.current_url)
        sleep(1)
    logout_action()
    driver.quit()