using Newtonsoft.Json;
namespace eShopAnalysis.Aggregator.Services.BackchannelDto
{
    /// <summary>
    /// request from aggregate write to StockInventory 
    /// in aggregate write AddStockReqTransAndIncreaseStockItems
    /// to increase stock item quantity when request stock item from stock provider
    /// </summary>
    public class StockIncreaseRequestDto
    {
        [JsonProperty]
        public Guid ProductModelId { get; set; }

        [JsonProperty]
        public int QuantityToIncrease { get; set; }

        [JsonConstructor]
        public StockIncreaseRequestDto(Guid productModelId, int quantityToIncrease)
        {
            ProductModelId = productModelId;
            QuantityToIncrease = quantityToIncrease;
        }
    }
}
