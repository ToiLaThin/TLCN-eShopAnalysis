export enum DiscountType {
    ByValue = 0,
    ByPercent = 1,
}
export enum Status
{
    Active = 0,
    Ended = 1
}
export interface ISaleItem {
    productId: string;
    productModelId: string;
    businessKey: string;
    discountType: DiscountType;
    discountValue: number;
    minOrderValueToApply?: number;//???
    dateAdded?: Date;
    dateEnded?: Date;
    couponStatus?: Status;
    rewardPointRequire?: number;

}