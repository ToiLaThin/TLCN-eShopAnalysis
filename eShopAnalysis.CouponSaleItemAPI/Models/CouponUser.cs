namespace eShopAnalysis.CouponSaleItemAPI.Models
{
    public class CouponUser
    {
        public Guid UserId { get; set; }

        public Guid CouponId { get; set; }

        public Coupon CouponUsed { get; set; }
    }
}
