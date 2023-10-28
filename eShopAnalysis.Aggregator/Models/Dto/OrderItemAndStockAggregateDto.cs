
using eShopAnalysis.Aggregator.Services.BackchannelDto;

namespace eShopAnalysis.Aggregator.Models.Dto
{
    public class ItemStockDto
    {
        public Guid ProductModelId { get; set; }

        public int CurrentQuantity { get; set; }
    }

    public class OrderItemsDto {
        public Guid OrderId { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public List<OrderItemQuantityDto> OrderItemsQty { get; set; }

        public double TotalPriceFinal { get; set; }
    }

    //the data sent to the frontend
    public class OrderItemAndStockAggregateDto
    {
        public IEnumerable<OrderItemsDto> OrderItems { get; set; }

        public Dictionary<string, int> ItemsStock { get; set; }
    }
}
