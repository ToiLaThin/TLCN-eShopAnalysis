namespace eShopAnalysis.ApiGateway.Services.BackchannelDto
{
    public enum PaymentMethod
    {
        COD = 0,
        Momo = 1,
        CreditCard = 2
    }
    //status is customerInfoConfirmed and payment method is COD can still be load on the UI for admin to handle
    public enum OrderStatus
    {
        CreatedDraft = 0,
        CustomerInfoConfirmed = 1,
        Checkouted = 2,
        StockConfirmed = 3,
        Refunded = 4,
        Cancelled = 5,
        Completed = 6
    }

    //response from cartOrder for aggregate into orderItems with stock
    public class OrderItemQuantityDto
    {
        public Guid ProductModelId { get; set; }

        public int Quantity { get; set; }
    }

    public class OrderItemsResponseDto
    {
        public Guid OrderId { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public List<OrderItemQuantityDto> OrderItemsQty { get; set; }

        public double TotalPriceFinal { get; set; }

        
    }
}
