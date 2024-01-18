using Newtonsoft.Json;

namespace eShopAnalysis.StockInventoryAPI.Dto.BackchannelDto
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
