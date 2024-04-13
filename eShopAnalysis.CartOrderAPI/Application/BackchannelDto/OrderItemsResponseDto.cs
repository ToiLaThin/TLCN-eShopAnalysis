using eShopAnalysis.CartOrderAPI.Domain.DomainModels.OrderAggregate;

namespace eShopAnalysis.CartOrderAPI.Services.BackchannelDto
{    
    //public enum PaymentMethod
    //{
    //    COD = 0,
    //    Momo = 1,
    //    CreditCard = 2
    //}
    ////status is customerInfoConfirmed and payment method is COD can still be load on the UI for admin to handle
    //public enum OrderStatus
    //{
    //    CreatedDraft = 0,
    //    CustomerInfoConfirmed = 1,
    //    Checkouted = 2,
    //    StockConfirmed = 3,
    //    Refunded = 4,
    //    Cancelled = 5,
    //    Completed = 6
    //}

    //response from cartOrder for aggregate into orderItems with stock
    public record OrderItemQuantityDto
    {
        public Guid ProductModelId { get; }

        public int Quantity { get; }
    }

    public record OrderItemsResponseDto
    {
        public Guid OrderId { get; }

        public OrderStatus OrderStatus { get; }

        public PaymentMethod PaymentMethod { get; }

        public List<OrderItemQuantityDto> OrderItemsQty { get; set; }

        public double TotalPriceFinal { get; }

        
    }
}
