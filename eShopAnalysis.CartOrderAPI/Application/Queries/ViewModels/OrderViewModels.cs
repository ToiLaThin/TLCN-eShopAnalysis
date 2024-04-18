using eShopAnalysis.CartOrderAPI.Domain.DomainModels.CartAggregate;
using eShopAnalysis.CartOrderAPI.Domain.DomainModels.OrderAggregate;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace eShopAnalysis.CartOrderAPI.Application.Queries
{
    public record OrderDraftViewModel
    {
        public Guid OrderId { get;  }

        public double SubTotal { get; }

        public double TotalDiscount { get; }
    }

    public record OrderViewModel
    {
        public Guid OrderId { get; set; }
    }

    public record AddressViewModel
    {
        public Guid OrderReferenceId { get; }
        public string Country { get; }
        public string CityOrProvinceOrPlace { get; }
        public string DistrictOrLocality { get; }
        public string PostalCode { get; }
        public string Street { get; }
        public string FullAddressName { get; }

        //require to materialize address view model
        public AddressViewModel()
        {
        }

        public AddressViewModel(Guid orderReferenceId, Address address)
        {
            OrderReferenceId = orderReferenceId;
            Country = address.Country;
            CityOrProvinceOrPlace = address.CityOrProvinceOrPlace;
            DistrictOrLocality = address.DistrictOrLocality;
            PostalCode = address.PostalCode;
            Street = address.Street;
            FullAddressName = address.FullAddressName;
        }

        public AddressViewModel(Address address) {
            Country = address.Country;
            CityOrProvinceOrPlace = address.CityOrProvinceOrPlace;
            DistrictOrLocality = address.DistrictOrLocality;
            PostalCode = address.PostalCode;
            Street = address.Street;
            FullAddressName = address.FullAddressName;
        }
    }

    public record OrderAggregateCartViewModel
    {
        public Guid OrderId { get; }
        public Guid CartId { get; }
        public Guid OrderBusinessKey { get; }

        public CartSummaryViewModel Cart { get; set; }
                                      
        public AddressViewModel? Address { get; set; }

        public string? PhoneNumber { get; }

        public PaymentMethod? PaymentMethod { get; }

        public OrderStatus OrderStatus { get; }

        public DateTime DateCreatedDraft { get; }

        public DateTime? DateCustomerInfoConfirmed { get; }

        public DateTime? DateCheckouted { get; }

        public DateTime? DateStockConfirmed { get; }

        public DateTime? DateRefunded { get; }

        public DateTime? DateCancelled { get;}
                                             
        public DateTime? DateCompleted { get;}

        //public int Revision { get; } this is not necessary
        public OrderAggregateCartViewModel(Order order)
        {
            OrderId = order.Id;
            CartId = order.CartId;
            OrderBusinessKey = order.BusinessKey;
            Cart = new CartSummaryViewModel(order.Cart);
            Address = new AddressViewModel(order.Id, order.Address);
            PhoneNumber = order.PhoneNumber;
            PaymentMethod = order.PaymentMethod;
            OrderStatus = order.OrdersStatus;
            DateCreatedDraft = order.DateCreatedDraft;
            DateCustomerInfoConfirmed = order.DateCustomerInfoConfirmed;
            DateCheckouted = order.DateCheckouted;
            DateStockConfirmed = order.DateStockConfirmed;
            DateRefunded = order.DateRefunded;
            DateCancelled = order.DateCancelled;
            DateCompleted = order.DateCompleted;
        }

        //***these consturctor is neccessary for OrderAggregateCartViewModel to be materialize by dapper.sql.builder****
        public OrderAggregateCartViewModel(
            Guid OrderId,
            Guid CartId,
            Guid OrderBusinessKey,
            OrderStatus OrderStatus,
            PaymentMethod PaymentMethod,
            string PhoneNumber,
            DateTime DateCreatedDraft,
            DateTime DateCustomerInfoConfirmed,
            DateTime DateCheckouted,
            DateTime DateStockConfirmed,
            DateTime DateRefunded,
            DateTime DateCancelled,
            DateTime DateCompleted)
        {
            //matchching name assign is not necessary, but must have this. = , so the data is not having default value or null, etc...
            this.OrderId = OrderId;
            this.CartId = CartId;
            this.OrderBusinessKey = OrderBusinessKey;
            //Cart = new CartSummaryViewModel(order.Cart);
            //Address = new AddressViewModel(order.Id, order.Address);
            this.PhoneNumber = PhoneNumber;
            this.PaymentMethod = PaymentMethod;
            this.OrderStatus = OrderStatus;
            this.DateCreatedDraft = DateCreatedDraft;
            this.DateCustomerInfoConfirmed = DateCustomerInfoConfirmed;
            this.DateCheckouted = DateCheckouted;
            this.DateStockConfirmed = DateStockConfirmed;
            this.DateRefunded = DateRefunded;
            this.DateCancelled = DateCancelled;
            this.DateCompleted = DateCompleted;
        }
    }

    public record CartSummaryViewModel
    {
        public Guid CartPrimaryKey { get; }

        public Guid UserId { get; }

        public bool HaveCouponApplied { get; }
        public Guid? CouponId { get; }
        public bool HaveAnySaleItem { get; }

        public DiscountType? CouponDiscountType { get; }

        //IF do not have value, then price default value is -1 eexxcept for TotalPriceOriginal and TotalPriceTotal

        public double CouponDiscountAmount { get; } 
        //in value, convert percent to value in case percentage is the discount type , for analytic purpose

        public double CouponDiscountValue { get; } 
        //in percent or value depend on the type

        public double TotalSaleDiscountAmount { get; }

        public double TotalPriceOriginal { get; } 
        //even if have any sale item, this is the price IF there is no sale item

        public double TotalPriceAfterSale { get; }

        public double TotalPriceAfterCouponApplied { get; } 
        //after sale item applied, continue apply coupon discount on the totalPriceAfterSale

        public double TotalPriceFinal { get; } 
        //might be equal priceAfterCouponApplied, or priceAfterSale, or Original

        public List<CartItemViewModel> Items { get; set; }

        public CartSummaryViewModel(Guid cartPrimaryKey,
            Guid userId,
            bool haveCouponApplied,
            Guid? couponId,
            bool haveAnySaleItem,
            DiscountType? couponDiscountType,
            double couponDiscountAmount,
            double totalSaleDiscountAmount,
            double totalPriceOriginal,
            double totalPriceAfterSale,
            double totalPriceAfterCouponApplied,
            double totalPriceFinal,
            IEnumerable<CartItemViewModel> items)
        {
            this.CartPrimaryKey = cartPrimaryKey;
            this.UserId = userId;
            this.HaveCouponApplied = haveCouponApplied;
            this.CouponId = couponId;
            this.HaveAnySaleItem = haveAnySaleItem;
            this.CouponDiscountType = couponDiscountType;
            this.CouponDiscountAmount = couponDiscountAmount;
            this.TotalSaleDiscountAmount = totalSaleDiscountAmount;
            this.TotalPriceOriginal = totalPriceOriginal;
            this.TotalPriceAfterSale = totalPriceAfterSale;
            this.TotalPriceAfterCouponApplied = totalPriceAfterCouponApplied;
            this.TotalPriceFinal = totalPriceFinal;
            this.Items = items.ToList();
        }

        public CartSummaryViewModel(
            Guid CartPrimaryKey,
            Guid UserId,
            bool HaveCouponApplied,
            Guid CouponId,
            bool HaveAnySaleItem,
            DiscountType? CouponDiscountType,
            double CouponDiscountAmount,
            double CouponDiscountValue,
            double TotalSaleDiscountAmount,
            double TotalPriceOriginal,
            double TotalPriceAfterSale,
            double TotalPriceAfterCouponApplied,
            double TotalPriceFinal)
        {
            this.CartPrimaryKey = CartPrimaryKey;
            this.UserId = UserId;
            this.HaveCouponApplied = HaveCouponApplied;
            this.CouponId = CouponId;
            this.HaveAnySaleItem = HaveAnySaleItem;
            this.CouponDiscountType = CouponDiscountType;
            this.CouponDiscountAmount = CouponDiscountAmount;
            this.CouponDiscountValue = CouponDiscountValue;
            this.TotalSaleDiscountAmount = TotalSaleDiscountAmount;
            this.TotalPriceOriginal = TotalPriceOriginal;
            this.TotalPriceAfterSale = TotalPriceAfterSale;
            this.TotalPriceAfterCouponApplied = TotalPriceAfterCouponApplied;
            this.TotalPriceFinal = TotalPriceFinal;
            //this.Items = items.ToList();
        }

        //this constructor make it easier to call
        public CartSummaryViewModel(CartSummary cartSummary)
        {
            this.CartPrimaryKey = cartSummary.Id;
            this.UserId = cartSummary.UserId;
            this.HaveCouponApplied = cartSummary.HaveCouponApplied;
            this.CouponId = cartSummary.CouponId;
            this.HaveAnySaleItem = cartSummary.HaveAnySaleItem;
            this.CouponDiscountType = cartSummary.CouponDiscountType;
            this.CouponDiscountAmount = cartSummary.CouponDiscountAmount;
            this.TotalSaleDiscountAmount = cartSummary.TotalSaleDiscountAmount;
            this.TotalPriceOriginal = cartSummary.TotalPriceOriginal;
            this.TotalPriceAfterSale = cartSummary.TotalPriceAfterSale;
            this.TotalPriceAfterCouponApplied = cartSummary.TotalPriceAfterCouponApplied;
            this.TotalPriceFinal = cartSummary.TotalPriceFinal;
            this.Items = cartSummary.Items.Select(cartItem => new CartItemViewModel(cartItem)).ToList();
        }
    }
    public record CartItemViewModel
    {
        public Guid ProductId { get; }
        public Guid ProductModelId { get; }
        public Guid CartItemBusinessKey { get; }
        public Guid CartId { get; }

        public Guid? SaleItemId { get; }

        public bool IsOnSale { get; }

        public DiscountType? SaleType { get; }

        public double SaleValue { get; }

        public int Quantity { get; }

        public double UnitPrice { get; }
        public double FinalPrice { get; }

        //currently each unit will receive the same sale value TODO, may add more rule on the quantity number to have this sale
        public double UnitAfterSalePrice { get; }
        public double FinalAfterSalePrice { get; }

        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string SubCatalogName { get; set; }
        
        public CartItemViewModel() { }
        public CartItemViewModel(
            Guid productId, 
            Guid productModelId, 
            Guid cartItemBusinessKey, 
            Guid cartId, 
            Guid? saleItemId, 
            bool isOnSale,
            DiscountType? saleType, 
            double saleValue,
            int quantity,
            double unitPrice,
            double finalPrice,
            double unitAfterSalePrice,
            double finalAfterSalePrice, string productName, string productImage, string subCatalogName)
        {
            ProductId = productId;
            ProductModelId = productModelId;
            CartItemBusinessKey = cartItemBusinessKey;
            CartId = cartId;
            SaleItemId = saleItemId;
            IsOnSale = isOnSale;
            SaleType = saleType;
            SaleValue = saleValue;
            Quantity = quantity;
            UnitPrice = unitPrice;
            FinalPrice = finalPrice;
            UnitAfterSalePrice = unitAfterSalePrice;
            FinalAfterSalePrice = finalAfterSalePrice;
            ProductName = productName;
            ProductImage = productImage;
            SubCatalogName = subCatalogName;
        }

        public CartItemViewModel(CartItem cartItem)
        {
            ProductId = cartItem.ProductId;
            ProductModelId = cartItem.ProductModelId;
            CartItemBusinessKey = cartItem.BusinessKey;
            CartId = cartItem.CartId;
            SaleItemId = cartItem.SaleItemId;
            IsOnSale = cartItem.IsOnSale;
            SaleType = cartItem.SaleType;
            SaleValue = cartItem.SaleValue;
            Quantity = cartItem.Quantity;
            UnitPrice = cartItem.UnitPrice;
            FinalPrice = cartItem.FinalPrice;
            UnitAfterSalePrice = cartItem.UnitAfterSalePrice;
            FinalAfterSalePrice = cartItem.FinalAfterSalePrice;
            ProductName = cartItem.ProductName;
            ProductImage = cartItem.ProductImage;
            SubCatalogName = cartItem.SubCatalogName;
        }
    }
}
