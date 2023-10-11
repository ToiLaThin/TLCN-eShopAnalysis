using eShopAnalysis.CouponSaleItemAPI.Models;

namespace eShopAnalysis.CouponSaleItemAPI.Dto
{
    public class SaleItemDto
    {
        public Guid SaleItemId { get; set; }
        public Guid ProductId { get; set; }
        public Guid ProductModelId { get; set; }
        public Guid BusinessKey { get; set; }

        public DiscountType DiscountType { get; set; }

        public double DiscountValue { get; set; }

        public DateTime DateAdded { get; set; }

        public DateTime DateEnded { get; set; }

        public Status SaleItemStatus { get; set; }

        public int RewardPointRequire { get; set; }
    }
}
