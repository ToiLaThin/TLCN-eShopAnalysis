
import pandas as pd
import json
from flows import order_confirm_flow
from random import randint, choices, sample
from datetime import datetime

f = open('user-occasion.json', 'r')
users_occasion = json.load(f)
for user in users_occasion:
    user_name = user['username']
    password = user['password']
    profiles = user['profiles']
    today = datetime.now()
    day_of_week = today.weekday()
    if day_of_week not in user["weekday"]:
        print(f"User {user_name} is not buy today, weekday: {day_of_week}, 0 - 6 is Monday - Sunday")
        continue
    num_profile_use = 1 #randint(1, len(profiles))
    profile_weights = [profile["weight"] for profile in profiles]
    profiles_use = choices(profiles, k=num_profile_use, weights=profile_weights)
    one_profile_use = profiles_use[0]
    purpose = one_profile_use['purpose']
    print(f"User {user_name} is buy with purpose: {purpose}")
    items = one_profile_use['items']
    order_confirm_flow(user_name, password, items)

f = open('user-frequent.json', 'r')
users_frequent = json.load(f)
for user in users_frequent:
    user_name = user['username']
    password = user['password']
    profiles = user['profiles']
    # pick one or many profile by random
    num_profile_use = 1 #randint(1, len(profiles))
    profile_weights = [profile["weight"] for profile in profiles]
    profiles_use = choices(profiles, k=num_profile_use, weights=profile_weights)
    one_profile_use = profiles_use[0]
    #purpose can be 
    # Cooking
    # Breakfast
    # Partying
    # Man Care
    # Woman Care
    # Infant Care
    # Pet Care
    # Snacks
    # Cleaning
    # Hybrid

    purpose = one_profile_use['purpose']
    print(f"User {user_name} is buy with purpose: {purpose}")
    items = one_profile_use['items']
    order_confirm_flow(user_name, password, items)



