using eShopAnalysis.CouponSaleItemAPI.Models;

namespace eShopAnalysis.CouponSaleItemAPI.Dto
{
    public class CouponDto
    {
        public Guid CouponId { get; set; }

        public string CouponCode { get; set; }

        public DiscountType DiscountType { get; set; }

        public double DiscountValue { get; set; }

        public double MinOrderValueToApply { get; set; }

        public DateTime DateAdded { get; set; }

        public DateTime DateEnded { get; set; }

        public Status CouponStatus { get; set; }

        public int RewardPointRequire { get; set; }
    }
}
