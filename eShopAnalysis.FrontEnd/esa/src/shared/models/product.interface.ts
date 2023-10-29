import { DiscountType } from "./saleItem.interface";

export enum CublicType {
    M = 0,
    V = 1,
    S = 2,
    N = 3,
}

export interface IProductModel {
    productModelId?: string;
    productModelThumbnails: string[];
    cublicType: CublicType;
    cublicValue: number;
    pricePerCublicValue?: number;
    cublicPrice?: number;
    price: number;
    isOnSaleModel?: boolean;
    saleItemId?: string;
    saleValueModel?: number;
    saleType?: DiscountType;
    priceOnSaleModel?: number;
}

export interface IProductInfo {
    productDescription: string;
    productBrand: string;
}

export interface IProduct {
    productId?: string;
    productName?: string;
    subCatalogId?: string;
    subCatalogName?: string;
    productCoverImage?: string;
    isOnSale?: boolean;
    productDisplaySaleValue?: number;
    productDisplaySaleType?: DiscountType;
    productDisplayPriceOnSale?: number;
    haveVariants?: boolean;
    havePricePerCublic?: boolean;
    revision?: number;
    businessKey?: string;
    productInfo?: IProductInfo;
    productModels: IProductModel[];
}