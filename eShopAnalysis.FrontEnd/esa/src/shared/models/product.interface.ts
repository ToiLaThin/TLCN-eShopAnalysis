import { IProductInfo } from "./productInfo.interface";
import { IProductModel } from "./productModel.interface";
import { DiscountType } from "./saleItem.interface";

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