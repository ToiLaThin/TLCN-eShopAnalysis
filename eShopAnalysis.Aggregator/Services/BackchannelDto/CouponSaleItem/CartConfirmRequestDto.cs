
namespace eShopAnalysis.Aggregator.Application.BackchannelDto
{
    public enum DiscountType
    {
        ByValue,
        ByPercent,
        NoDiscount
    }

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

        public int Quantity { get; set; }

        public double UnitPrice { get; set; }
        public double FinalPrice { get; set; }

        public double UnitAfterSalePrice { get; set; }
        public double FinalAfterSalePrice { get; set; }
    }

    //request from frontend
    public class CartConfirmRequestDto
    {
        public IEnumerable<CartItem> CartItems { get; set; }

        public Guid UserId { get; set; }

        //not require
        public string? CouponCode { get; set; }
    }

    //Request to send CartOrder to add cart once determine if we have coupon or not
    public class CartConfirmRequestToCartApiDto
    {
        public IEnumerable<CartItem> CartItems { get; set; }

        public Guid UserId { get; set; }

        //not require
        public CouponDto? Coupon { get; set; }
    }
}
