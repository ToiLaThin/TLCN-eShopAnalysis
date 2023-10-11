export enum DiscountType {
    ByValue = 0,
    ByPercent = 1,
    NoDiscount = 2, //must match with backend

}
export enum Status
{
    Active = 0,
    Ended = 1
}
export interface ISaleItem {
    saleItemId?: string; //for post
    productId: string;
    productModelId: string;
    businessKey: string;
    discountType: DiscountType;
    discountValue: number;
    dateAdded?: Date;
    dateEnded?: Date;
    saleItemStatus?: Status;
    rewardPointRequire?: number;

}