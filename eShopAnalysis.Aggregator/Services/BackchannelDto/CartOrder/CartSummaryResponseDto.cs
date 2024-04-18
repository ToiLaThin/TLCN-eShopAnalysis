using Newtonsoft.Json;

namespace eShopAnalysis.Aggregator.Services.BackchannelDto
{
    /// <summary>
    /// Response from CartOrderApi when calling backchannel/addCart after successfully add cart and order
    /// </summary>
    public class CartSummaryResponseDto
    {
            [JsonProperty]
            public Guid Id { get; set; }

            [JsonProperty]
            public Guid UserId { get; set; }

            //public Guid CartId { get; set; } Id in the base abstract class

            //public CartStatus CartStatus { get; set; } //TODO forgot to add, plese add this in the next migrations,default value is create

            //public DateTime DateAdded { get; set; }

            //public DateTime DateCompleted { get; set; }

            [JsonProperty]
            public bool HaveCouponApplied { get; private set; }

            [JsonProperty]
            public Guid? CouponId { get; private set; }

            [JsonProperty]
            public bool HaveAnySaleItem { get; private set; }

            [JsonProperty]
            public DiscountType? CouponDiscountType { get; private set; }

            //IF do not have value, then price default value is -1 eexxcept for TotalPriceOriginal and TotalPriceTotal
            [JsonProperty]
            public double CouponDiscountAmount { get; private set; } //in value, convert percent to value in case percentage is the discount type , for analytic purpose

            [JsonProperty]
            public double CouponDiscountValue { get; private set; } //in percent or value depend on the type

            [JsonProperty]
            public double TotalSaleDiscountAmount { get; private set; }

            [JsonProperty]
            public double TotalPriceOriginal { get; private set; } //even if have any sale item, this is the price IF there is no sale item

            [JsonProperty]
            public double TotalPriceAfterSale { get; private set; }

            [JsonProperty]
            public double TotalPriceAfterCouponApplied { get; private set; } //after sale item applied, continue apply coupon discount on the totalPriceAfterSale

            [JsonProperty]
            public double TotalPriceFinal { get; private set; } //might be equal priceAfterCouponApplied, or priceAfterSale, or Original

            [JsonProperty]
            public List<CartItem> Items { get; private set; }
        }
}
