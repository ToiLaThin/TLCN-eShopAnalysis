using Newtonsoft.Json;
namespace eShopAnalysis.Aggregator.Services.BackchannelDto
{
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
