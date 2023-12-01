import pandas as pd
from pandas import DataFrame

from jobs.utils.fkey_utils import *

def transform_product_df_to_product_df_with_fkey(product_df: DataFrame) -> DataFrame:
    """Convert product dataframe to product dataframe with fkey"""
    columns_to_keep = ['ProductId', 'ProductName', 'BrandKey', 'UsageInstructionTypeKey', 'SubCatalogKey'\
                       , "PreserveInstructionTypeKey", "HaveModels", "HavePricePerCublic", "BusinessKey", "Revision"]
    product_df_to_have_fkey = product_df.copy()
    dims_lookup = DimLookUp()
    brand_series = product_df_to_have_fkey['ProductBrandName']
    subcatalog_series = product_df_to_have_fkey['ProductSubCatalogName']
    usage_instruction_series = product_df_to_have_fkey['ProductUsageInstruction']
    preserve_instruction_series = product_df_to_have_fkey['ProductPreserveInstruction']

    brand_fkey_series = brand_series.apply(lambda x: map_product_brand_to_fkey(x, dims_lookup))
    subcatalog_fkey_series = subcatalog_series.apply(lambda x: map_product_subcatalog_to_fkey(x, dims_lookup))
    usage_instruction_fkey_series = usage_instruction_series.apply(lambda x: map_product_usage_instruction_to_fkey(x))
    preserve_instruction_fkey_series = preserve_instruction_series.apply(lambda x: map_product_preserve_instruction_to_fkey(x))
    
    product_df_to_have_fkey['BrandKey'] = brand_fkey_series
    product_df_to_have_fkey['SubCatalogKey'] = subcatalog_fkey_series
    product_df_to_have_fkey['UsageInstructionTypeKey'] = usage_instruction_fkey_series
    product_df_to_have_fkey['PreserveInstructionTypeKey'] = preserve_instruction_fkey_series
    product_df_to_have_fkey = product_df_to_have_fkey[columns_to_keep] # [[]] return dataframe, [] return series -> return dataframe
    del dims_lookup # delete the object to free up memory
    return product_df_to_have_fkey