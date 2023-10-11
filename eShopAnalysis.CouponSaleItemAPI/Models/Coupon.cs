using System.ComponentModel.DataAnnotations;

namespace eShopAnalysis.CouponSaleItemAPI.Models
{
    public enum DiscountType
    {
        ByValue,
        ByPercent,
        NoDiscount
    }

    public enum Status
    {
        Active,
        Ended
    }
    public class Coupon
    {
        public Guid CouponId { get; set; }

        public string CouponCode { get; set; }

        public DiscountType DiscountType { get; set; }

        //[Range(0, int.MaxValue)] can use this but if used fluentApi, use it
        public double DiscountValue { get; set; }

        public double MinOrderValueToApply { get; set; }

        public DateTime DateAdded { get; set; }

        public DateTime DateEnded { get; set; }

        public Status CouponStatus { get; set; }

        public int RewardPointRequire { get; set; }

    }
}
