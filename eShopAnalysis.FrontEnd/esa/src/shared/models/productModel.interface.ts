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
    saleValueModel?: number;
    saleType?: DiscountType;
    priceOnSaleModel?: number;
}