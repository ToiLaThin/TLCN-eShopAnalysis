namespace eShopAnalysis.Aggregator.Services.BackchannelDto
{
    public enum CouponDiscountType
    {
        ByPercent = 0,
        ByValue = 1 //or currency
    }

    /// <summary>
    /// request from aggregator write to CustomerLoyaltyProgramAPI (at reward transaction controller)
    /// in CheckCouponAndAddCart
    /// to change reward point balance for apply coupon 
    /// </summary>
    public class RewardTransactionForApplyCouponAddRequestDto
    {
        public Guid UserId { get; set; }

        public CouponDiscountType DiscountType { get; set; }

        public double DiscountValue { get; set; }

        public int PointTransition { get; set; }

    }

    //helper static class having helper adapter method to resolve type config
    public static class RewardTransactionHelperAdapter {
        public static CouponDiscountType ToCouponDiscountTypeAdapter(DiscountType discountType)
        {
            if (discountType == DiscountType.NoDiscount) {
                throw new ArgumentException(nameof(discountType));
            }
            switch (discountType)
            {
                case DiscountType.ByPercent:
                    return CouponDiscountType.ByPercent;
                case DiscountType.ByValue:
                    return CouponDiscountType.ByValue;
                default:
                    return CouponDiscountType.ByValue;
            }
        }
    }
}
