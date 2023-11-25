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


//lazy load request
export enum ProductPerPage {
    Sixteen = 16,
    ThirtyTwo = 32,
    FortyEight = 48,
}

export enum SortBy {
    Id = 0,
    Price = 1,
    Name = 2
}

export enum FilterBy {
    SubCatalogs = 0,
    Price = 1
}

export enum OrderType {
    Ascending = 0,
    Descending = 1
}

export interface IPriceMeta {
    fromPrice: number;
    toPrice: number;
}

export interface IFilterRequest {
    filterBy: FilterBy;
    Meta: string;
}

export interface IProductLazyLoadRequest {
    pageOffset: number;
    productPerPage: number;
    sortBy: SortBy;
    orderType: OrderType;
    filterRequests: IFilterRequest[];
}

export interface IPaginatedProduct {
    products: IProduct[];
    pageNumber: number;
    pageCount: number;
}