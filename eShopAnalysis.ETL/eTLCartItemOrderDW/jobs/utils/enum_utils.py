from enum import Enum
import pandas as pd
class OrderStatus(Enum):
    CreatedDraft = 0,
    CustomerInfoConfirmed = 1,
    Checkouted = 2,
    StockConfirmed = 3,
    Refunded = 4,
    Cancelled = 5,
    Completed = 6

class PaymentMethod(Enum):
    COD = 0,
    Momo = 1,
    CreditCard = 2

class DiscountType(Enum):
    ByValue = 0,
    ByPercent = 1,
    NoDiscount = 2

class Address(Enum):
    Quan1 = 0,
    QuanThuDuc = 1,    
    QuanBinhThanh = 2,
    QuanTanBinh = 3,


def generate_order_status_df():
    order_status_dict = {}
    for i, status in enumerate(OrderStatus, start=0):
        print(type(status.name))
        status_num = status.value[0] if type(status.value) is tuple else status.value
        order_status_dict[status_num] = status.name
    df_order_status = pd.DataFrame.from_dict(order_status_dict, orient='index', columns=['OrderStatus'])
    df_order_status.to_csv('resources/order_status.csv', index=False)
    return df_order_status

def generate_payment_method_df():
    payment_method_dict = {}
    for i, method in enumerate(PaymentMethod, start=0):
        payment_method_num = method.value[0] if type(method.value) is tuple else method.value
        payment_method_dict[payment_method_num] = method.name
    df_payment_method = pd.DataFrame.from_dict(payment_method_dict, orient='index', columns=['PaymentMethod'])
    df_payment_method.to_csv('resources/payment_method.csv', index=False)
    return df_payment_method

def generate_discount_type_df():
    discount_type_dict = {}
    for i, discount_type in enumerate(DiscountType, start=0):
        discount_type_num = discount_type.value[0] if type(discount_type.value) is tuple else discount_type.value
        discount_type_dict[discount_type_num] = discount_type.name
    df_discount_type = pd.DataFrame.from_dict(discount_type_dict, orient='index', columns=['DiscountType'])
    df_discount_type.to_csv('resources/discount_type.csv', index=False)
    return df_discount_type

def generate_address_df():
    address_dict = {}
    for i, address in enumerate(Address, start=0):
        address_num = address.value[0] if type(address.value) is tuple else address.value
        address_dict[address_num] = address.name
    df_address = pd.DataFrame.from_dict(address_dict, orient='index', columns=['Address'])
    df_address.to_csv('resources/address.csv', index=False)
    return df_address