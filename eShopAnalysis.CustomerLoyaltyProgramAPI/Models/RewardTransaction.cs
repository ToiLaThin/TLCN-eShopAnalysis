namespace eShopAnalysis.CustomerLoyaltyProgramAPI.Models
{
    public enum Reason
    {
        Order = 0,
        ApplyCoupon = 1
    }

    public enum CouponDiscountType
    {
        ByPercent = 0,
        ByValue= 1 //or currency
    }

    public class OriginJson
    {
        public Reason Reason { get; set; }

        public double? OrderPrice { get; set; } //not null if reason is order

        // not null if reason is Apply Coupon
        public CouponDiscountType? DiscountType { get; set; } 

        public double? DiscountValue { get; set; }
    }

    public class RewardTransaction
    {
        public Guid RewardTransactionId { get; set; }

        public Guid UserId { get; set; }

        public int PointTransition { get; set; }

        public DateTime DateTransition { get; set; }

        public int PointBeforeTransaction { get; set; }

        public int PointAfterTransaction { get; set; }
        
        //json, config in entity type config
        public OriginJson Origin { get; set; }
    }
}
