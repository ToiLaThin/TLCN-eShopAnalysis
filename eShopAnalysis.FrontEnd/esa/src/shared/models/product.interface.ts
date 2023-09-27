import { IProductInfo } from "./productInfo.interface";
import { IProductModel } from "./productModel.interface";

export interface IProduct {
    productId?: string;
    productName?: string;
    subCatalogId?: string;
    subCatalogName?: string;
    productCoverImage?: string;
    isOnSale?: boolean;
    salePercent?: number;
    priceOnSale?: number;
    haveVariants?: boolean;
    havePricePerCublic?: boolean;
    revision?: number;
    businessKey?: string;
    productInfo?: IProductInfo;
    productModels?: IProductModel[];
}