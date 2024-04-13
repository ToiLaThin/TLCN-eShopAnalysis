using Newtonsoft.Json;

namespace eShopAnalysis.Aggregator.Services.BackchannelDto.CartOrder
{    
    public record AddressViewModel
    {
        [JsonProperty]
        public Guid OrderReferenceId { get; set; }
        [JsonProperty]
        public string Country { get; set;  }
        [JsonProperty]
        public string CityOrProvinceOrPlace { get; set; }
        [JsonProperty]
        public string DistrictOrLocality { get; set; }
        [JsonProperty]
        public string PostalCode { get; set; }
        [JsonProperty]
        public string Street { get; set; }
        [JsonProperty]
        public string FullAddressName { get; set; }
    }

    /// <summary>
    /// The response from cartOrder Api to get info about order that we just create from cartId. ALL PROP SHOULD HAVE SET; (DIFF FROM THE MODEL IN CARTORDER API). for the data is not null or have the default value
    /// </summary>
    public record OrderAggregateCartResponseDto
    {

        [JsonProperty]
        public Guid OrderId { get; set; }

        [JsonProperty]
        public Guid CartId { get; set; }

        [JsonProperty]
        public Guid OrderBusinessKey { get; set; }

        [JsonProperty]
        public CartSummaryViewModel Cart { get; set; }

        [JsonProperty]
        public AddressViewModel? Address { get; set; }

        [JsonProperty]
        public string? PhoneNumber { get; set; }

        [JsonProperty]
        public PaymentMethod? PaymentMethod { get; set;}

        [JsonProperty]
        public OrderStatus OrderStatus { get; set;}

        [JsonProperty]
        public DateTime DateCreatedDraft { get; set;}

        [JsonProperty]
        public DateTime? DateCustomerInfoConfirmed { get; set;}

        [JsonProperty]
        public DateTime? DateCheckouted { get; set;}

        [JsonProperty]
        public DateTime? DateStockConfirmed { get; set;}

        [JsonProperty]
        public DateTime? DateRefunded { get; set;}

        [JsonProperty]
        public DateTime? DateCancelled { get; set;}

        
        public DateTime? DateCompleted { get; set;}

        //public int Revision { get; } this is not necessary
    }

    public record CartSummaryViewModel
    {
        [JsonProperty]
        public Guid CartPrimaryKey { get; set; }

        [JsonProperty]
        public Guid UserId { get; set; }

        [JsonProperty]
        public bool HaveCouponApplied { get; set; }
        [JsonProperty]
        public Guid? CouponId { get; set; }
        [JsonProperty]
        public bool HaveAnySaleItem { get; set; }

        [JsonProperty]
        public DiscountType? CouponDiscountType { get; set; }

        //IF do not have value, then price default value is -1 eexxcept for TotalPriceOriginal and TotalPriceTotal

        [JsonProperty]
        public double CouponDiscountAmount { get; set; }
        //in value, convert percent to value in case percentage is the discount type , for analytic purpose

        [JsonProperty]
        public double CouponDiscountValue { get; set; }
        //in percent or value depend on the type

        [JsonProperty]
        public double TotalSaleDiscountAmount { get; set; }

        [JsonProperty]
        public double TotalPriceOriginal { get; set; }
        //even if have any sale item, this is the price IF there is no sale item

        [JsonProperty]
        public double TotalPriceAfterSale { get; set; }

        [JsonProperty]
        public double TotalPriceAfterCouponApplied { get; set; }
        //after sale item applied, continue apply coupon discount on the totalPriceAfterSale

        [JsonProperty]
        public double TotalPriceFinal { get; set; }
        //might be equal priceAfterCouponApplied, or priceAfterSale, or Original

        [JsonProperty]
        public List<CartItemViewModel> Items { get; set;  }
    }
    public record CartItemViewModel
    {
        [JsonProperty]
        public Guid ProductId { get; set; }

        [JsonProperty]
        public Guid ProductModelId { get; set; }

        [JsonProperty]
        public Guid CartItemBusinessKey { get; set; }

        [JsonProperty]
        public Guid CartId { get; set; }

        [JsonProperty]
        public Guid? SaleItemId { get; set; }

        [JsonProperty]
        public bool IsOnSale { get; set;  }

        [JsonProperty]
        public DiscountType? SaleType { get; set; }

        [JsonProperty]
        public double SaleValue { get; set; }

        [JsonProperty]
        public int Quantity { get; set; }

        [JsonProperty]
        public double UnitPrice { get; set; }

        [JsonProperty]
        public double FinalPrice { get; set;  }


        //currently each unit will receive the same sale value TODO, may add more rule on the quantity number to have this sale
        [JsonProperty]
        public double UnitAfterSalePrice { get; set; }

        [JsonProperty]
        public double FinalAfterSalePrice { get; set; }
    }
}
