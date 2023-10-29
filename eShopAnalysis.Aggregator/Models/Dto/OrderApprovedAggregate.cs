using Newtonsoft.Json;

namespace eShopAnalysis.Aggregator.Models.Dto
{
    //can also used for backchannel too
    public class StockDecreaseRequestDto
    {
        [JsonProperty]
        public Guid ProductModelId { get; set; }

        [JsonProperty]
        public int QuantityToDecrease { get; set; }

        [JsonConstructor]
        public StockDecreaseRequestDto(Guid productModelId, int quantityToDecrease)
        {
            ProductModelId = productModelId;
            QuantityToDecrease = quantityToDecrease;
        }

        public StockDecreaseRequestDto() {}
    }
    public class OrderApprovedAggregate
    {
        public Guid OrderId { get; set; }

        public IEnumerable<StockDecreaseRequestDto> OrderItemsStockToChange { get; set; }
    }
}
