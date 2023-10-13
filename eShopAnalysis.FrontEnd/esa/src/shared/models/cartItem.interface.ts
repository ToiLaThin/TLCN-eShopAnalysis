import { DiscountType } from "./saleItem.interface";

export interface ICartItem {
    cartId?: string; //for receiving
    productId?: string;
    productModelId?: string;
    businessKey?: string;
    isOnSale?: boolean;
    saleType?: DiscountType;
    saleValue?: number;
    quantity: number;
    unitPrice: number;
    finalPrice: number;
    unitAfterSalePrice?: number;
    finalAfterSalePrice?: number;        
}

export interface ICartConfirmRequest {
    cartItems: ICartItem[];
    userId: string;
}