using Newtonsoft.Json;

namespace eShopAnalysis.Aggregator.Services.BackchannelDto
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

    //json prop is required for the model to be serialized or deserialized correctly
    //if not , there will be error
    public class OrderItemsResponseDto
    {
        [JsonProperty]
        public Guid OrderId { get; set; }

        [JsonProperty]
        public OrderStatus OrderStatus { get; set; }

        [JsonProperty]
        public PaymentMethod PaymentMethod { get; set; }

        [JsonProperty]
        public List<OrderItemQuantityDto> OrderItemsQty { get; set; }

        [JsonProperty]
        public double TotalPriceFinal { get; set; }

        [JsonConstructor]
        public OrderItemsResponseDto(
            Guid orderId, 
            OrderStatus orderStatus, 
            PaymentMethod paymentMethod, 
            List<OrderItemQuantityDto> orderItemsQty, 
            double totalPriceFinal)
        {
            this.OrderId = orderId;
            this.PaymentMethod = paymentMethod;
            this.OrderStatus = orderStatus;
            this.TotalPriceFinal = totalPriceFinal;
            this.OrderItemsQty = orderItemsQty;
        }
        
    }
}
