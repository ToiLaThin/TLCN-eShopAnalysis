using Newtonsoft.Json;

namespace eShopAnalysis.Aggregator.Services.BackchannelDto
{
    /// <summary>
    /// request from aggregate write to StockInventory 
    /// in aggregate write ApproveOrdersAndModifyStocks
    /// to decrease stock item quantity when approve order
    /// </summary>
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

        public StockDecreaseRequestDto() { }
    }
}
