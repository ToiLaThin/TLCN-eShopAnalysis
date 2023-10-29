using Newtonsoft.Json;

namespace eShopAnalysis.StockInventoryAPI.Dto.BackchannelDto
{
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
    }
}
