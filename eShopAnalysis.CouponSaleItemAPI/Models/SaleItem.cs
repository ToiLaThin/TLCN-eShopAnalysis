namespace eShopAnalysis.CouponSaleItemAPI.Models
{
    public class SaleItem
    {
        //because if and item model is  on sales multiple time, id, modelId, and business key might not be enough
        public Guid SaleItemId { get; set; }
        public Guid ProductId { get; set; }
        public Guid ProductModelId { get; set; }
        public Guid BusinessKey { get; set; }

        public DiscountType DiscountType { get; set; }

        public double DiscountValue { get; set; }

        public double MinOrderValueToApply { get; set; }

        public DateTime DateAdded { get; set; }

        public DateTime DateEnded { get; set; }

        public Status CouponStatus { get; set; }

        public int RewardPointRequire { get; set; }
    }
}
