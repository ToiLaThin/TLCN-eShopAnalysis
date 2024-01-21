namespace eShopAnalysis.Aggregator.Services.BackchannelDto
{
    public enum Reason
    {
        Order = 0,
        ApplyCoupon = 1
    }

    public class OriginJson
    {
        public Reason Reason { get; set; }

        public double? OrderPrice { get; set; } //not null if reason is order

        // not null if reason is Apply Coupon
        public CouponDiscountType? DiscountType { get; set; }

        public double? DiscountValue { get; set; }
    }

    /// <summary>
    /// response from customer loyalty program api (at reward transaction controller)
    /// </summary>
    public class RewardTransactionDto
    {
        public Guid RewardTransactionId { get; set; }

        public Guid UserId { get; set; }

        public int PointTransition { get; set; }

        public DateTime DateTransition { get; set; }

        public int PointBeforeTransaction { get; set; }

        public int PointAfterTransaction { get; set; }

        public OriginJson Origin { get; set; }
    }
}
