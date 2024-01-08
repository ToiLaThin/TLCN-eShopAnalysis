using eShopAnalysis.CouponSaleItemAPI.Utilities.Prototype;

namespace eShopAnalysis.CouponSaleItemAPI.Models
{
    public class SaleItem : ClonableObject
    {
        //because if and item model is  on sales multiple time, id, modelId, and business key might not be enough
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


        //create a new row from existing instance of saleItem
        protected SaleItem(SaleItem sourceClone) : base(sourceClone){
            //after this clone constructor, change the saleItemId to newSaleItemId
            this.SaleItemId = sourceClone.SaleItemId;
            this.ProductId = sourceClone.ProductId;
            this.ProductModelId = sourceClone.ProductModelId;
            this.BusinessKey = sourceClone.BusinessKey;
            this.DiscountType = sourceClone.DiscountType;
            this.DiscountValue = sourceClone.DiscountValue;
            this.DateAdded = sourceClone.DateAdded;
            this.DateEnded = sourceClone.DateEnded;
            this.SaleItemStatus = sourceClone.SaleItemStatus;
            this.RewardPointRequire = sourceClone.RewardPointRequire;
        }
        public override ClonableObject Clone()
        {
            return new SaleItem(this);
        }

        //require to avoid error: unable to find suitable constructor, we must ensure it have a parameterless constructor
        public SaleItem()
        {

        }
    }
}
