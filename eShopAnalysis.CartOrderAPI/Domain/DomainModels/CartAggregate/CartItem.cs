using eShopAnalysis.CartOrderAPI.Domain.SeedWork;

namespace eShopAnalysis.CartOrderAPI.Domain.DomainModels.CartAggregate
{
    public enum DiscountType
    {
        ByValue,
        ByPercent,
        NoDiscount
    }

    //can use record for strongly type but i use value obj for ef core integration support
    //immutability , weak entity, identified by its value
    //public class CartItemId : ValueObject
    //{

    //    public Guid ProductId { get; set; }
    //    public Guid ProductModelId { get; set; }
    //    public Guid BusinessKey { get; set; }
    //    public Guid CartId { get; set; }
    //    private CartItemId(Guid pId,
    //                      Guid pMId,
    //                      Guid businessKey,
    //                      Guid cId)
    //    {
    //        ProductId = pId;
    //        ProductModelId = pMId;
    //        BusinessKey = businessKey;
    //        CartId = cId;
    //    }

    //    public CartItemId Build(Guid pId,
    //                      Guid pMId,
    //                      Guid businessKey,
    //                      Guid cId)
    //    {
    //        //check if those valid or not then call the private constuctor
    //        var newResulCartItemId = new CartItemId(pId, pMId, businessKey, cId);
    //        return newResulCartItemId;
    //    }

    //    protected override IEnumerable<object> GetMemberValues()
    //    {
    //        yield return ProductId;
    //        yield return ProductModelId;
    //        yield return BusinessKey;
    //        yield return CartId;
    //    }
    //}
    public class CartItem
    {
        public Guid ProductId { get; set; }
        public Guid ProductModelId { get; set; }
        public Guid BusinessKey { get; set; }
        public Guid CartId { get; private set; }

        public Guid? SaleItemId { get; set; }

        public bool IsOnSale { get; set; }

        public DiscountType? SaleType { get; set; }

        public double SaleValue { get; set; }

        public int Quantity{ get; set;}

        public double UnitPrice { get; set; }
        public double FinalPrice { get; set; }

        //currently each unit will receive the same sale value TODO, may add more rule on the quantity number to have this sale
        public double UnitAfterSalePrice { get; set; }
        public double FinalAfterSalePrice { get; set; }

        public string ProductName { get; set; }

        public string ProductImage {  get; set; }

        public string SubCatalogName { get; set; }


        //no longer necessary since we config foreign key constraint in CartSummaryEntityTypeConfiguration.cs
        public CartItem MarkBelongToCartWithId(Guid cartId)
        {
            this.CartId = cartId;
            return this;
        }
    }
}
