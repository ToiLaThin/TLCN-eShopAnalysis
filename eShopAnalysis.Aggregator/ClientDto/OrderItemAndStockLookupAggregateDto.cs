using eShopAnalysis.Aggregator.Services.BackchannelDto;

namespace eShopAnalysis.Aggregator.ClientDto
{ 
    public class OrderItemsDto
    {
        public Guid OrderId { get; set; }

        public Guid UserId { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public List<OrderItemQuantityDto> OrderItemsQty { get; set; }

        public double TotalPriceFinal { get; set; }
    }

    /// <summary>
    /// response from Aggregate to frontend
    /// contain info about order item & its stock to approve order
    /// </summary>
    public class OrderItemAndStockLookupAggregateDto
    {
        public IEnumerable<OrderItemsDto> OrderItems { get; set; }

        public IEnumerable<ProductModelInfoWithStockAggregateDto> StockLookupItems { get; set; }
    }
}
