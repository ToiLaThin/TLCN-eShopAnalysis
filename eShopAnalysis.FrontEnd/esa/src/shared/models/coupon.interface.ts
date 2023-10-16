import { DiscountType, Status } from "./saleItem.interface";

export interface ICoupon {
    couponId?: string;
    couponCode: string;
    discountType: DiscountType;
    discountValue: number;
    minOrderValueToApply: number;
    dateAdded?: Date;
    dateEnded?: Date;
    couponStatus?: Status;
    rewardPointRequire?: number;
}