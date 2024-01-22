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

    /// <summary>
    /// is used by both ClientDto & BackchannelDto
    /// response from cartOrder for aggregate into orderItems with stock in ClientDto.OrderItemAndStockAggregateDto.OrderItems
    /// </summary>
    public class OrderItemQuantityDto
    {
        public Guid ProductModelId { get; set; }
        public int Quantity { get; set; }
    }

    //json prop is required for the model to be serialized or deserialized correctly
    //if not , there will be error
    /// <summary>
    /// this is response from CartOrder Backchannel to get to approved order
    /// used in GetOrderToApprovedWithStock to construct OrderItemDto in aggregate model to response to Client
    /// </summary>
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
            OrderId = orderId;
            PaymentMethod = paymentMethod;
            OrderStatus = orderStatus;
            TotalPriceFinal = totalPriceFinal;
            OrderItemsQty = orderItemsQty;
        }

    }
}
