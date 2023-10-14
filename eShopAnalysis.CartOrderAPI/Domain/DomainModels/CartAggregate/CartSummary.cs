using Azure.Core;
using eShopAnalysis.CartOrderAPI.Domain.SeedWork;
using System.Runtime.CompilerServices;

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

        private CartSummary(Guid id) : base(id) { }

        private CartSummary(Guid cartId, Guid userId) : base(cartId)
        {
            //where cart got its id,only then it can passed this id to cart item
            this.UserId = userId;
            this.Items = new List<CartItem>();
        }

        private void AddToThisItem(CartItem itemToAdd)
        {
            //itemToAdd.MarkBelongToCartWithId(this.Id); //this is not necessary, since we config foreign key in entity type configuration, do not need to explicitly set it
            this.Items.Add(itemToAdd);
            //kt item hien tai co dc sale ko, neu co thi lay cai sale id do gan vo, cap nhat gia saleDiscountAmount, Price after sale
            if (itemToAdd.SaleType != DiscountType.NoDiscount)
            {
                if (!this.HaveAnySaleItem) //first sale item it have in cart
                {
                    this.HaveAnySaleItem = true;
                    this.TotalSaleDiscountAmount = 0;                    
                }
                //no matter first time or not , we still add the total sale discount amount since this is a sale itme
                this.TotalSaleDiscountAmount += itemToAdd.FinalPrice - itemToAdd.FinalAfterSalePrice;
            }
            this.TotalPriceOriginal += itemToAdd.UnitPrice * itemToAdd.Quantity;
        }

        public static CartSummary CreateCartSummaryFromItems(Guid cartGeneratedId,Guid buyerId, IEnumerable<CartItem> itemToAdd)
        {
            CartSummary cartSummary = new CartSummary(cartGeneratedId, buyerId);
            foreach (var cartItemToAdd in itemToAdd)
            {
                cartSummary.AddToThisItem(cartItemToAdd);
            }
            if (cartSummary.HaveAnySaleItem)
            {
                cartSummary.TotalPriceAfterSale = cartSummary.TotalPriceOriginal - cartSummary.TotalSaleDiscountAmount;
                //TODO coupon check
                cartSummary.TotalPriceFinal = cartSummary.TotalPriceAfterSale;
            }
            return cartSummary;
        }

    }
}
