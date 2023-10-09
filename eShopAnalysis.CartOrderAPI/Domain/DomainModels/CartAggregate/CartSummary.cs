using eShopAnalysis.CartOrderAPI.Domain.SeedWork;

namespace eShopAnalysis.CartOrderAPI.Domain.DomainModels.CartAggregate
{
    public enum CartStatus
    {
        Created, //when the order is created
        Abbandoned,
        Completed
    }

    public sealed class CartSummary : AggregateRoot
    {
        //public Guid CartId { get; set; } Id in the base abstract class
        public Guid UserId { get; set; }

        //public CartStatus CartStatus { get; set; } //TODO forgot to add, plese add this in the next migrations,default value is create

        //public DateTime DateAdded { get; set; }

        //public DateTime DateCompleted { get; set; }

        public bool HaveCouponApplied { get; set; }
        public Guid? CouponId { get;}
        public bool HaveAnySaleItem { get; set; }
        
        public DiscountType? CouponDiscountType{ get; set; }

        //IF do not have value, then price default value is -1 eexxcept for TotalPriceOriginal and TotalPriceTotal

        public double CouponDiscountAmount { get; set;} //in value, convert percent to value in case percentage is the discount type , for analytic purpose

        public double CouponDiscountValue { get; set; } //in percent or value depend on the type

        public double TotalSaleDiscountAmount { get; set; }

        public double TotalPriceOriginal { get; set; } //even if have any sale item, this is the price IF there is no sale item

        public double TotalPriceAfterSale { get; set; }

        public double TotalPriceAfterCouponApplied { get; set; } //after sale item applied, continue apply coupon discount on the totalPriceAfterSale

        public double TotalPriceFinal { get; set;} //might be equal priceAfterCouponApplied, or priceAfterSale, or Original

        public List<CartItem> Items { get; set; }

        public CartSummary(Guid id) : base(id) { }

        public CartSummary(Guid cartId, Guid userId) : base(cartId)
        {
            this.UserId = userId;
            this.Items = new List<CartItem>();
        }

        public void AddToThisItem(CartItem itemToAdd)
        {
            //itemToAdd.MarkBelongToCartWithId(this.Id); //this is not necessary, since we config foreign key in entity type configuration, do not need to explicitly set it
            this.Items.Add(itemToAdd);
            this.TotalPriceOriginal += itemToAdd.UnitPrice * itemToAdd.Quantity;
            this.TotalPriceFinal = this.TotalPriceOriginal; //TO DO change these logic as new business rule introduced
        }

    }
}
