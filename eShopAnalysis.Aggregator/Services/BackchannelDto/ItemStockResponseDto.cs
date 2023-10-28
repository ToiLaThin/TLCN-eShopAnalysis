using Newtonsoft.Json;

namespace eShopAnalysis.Aggregator.Services.BackchannelDto
{
    //json prop is required for the model to be serialized or deserialized correctly
    //if not , there will be error
    public class ItemStockResponseDto
    {
        [JsonProperty]
        public Guid ProductModelId { get; set; }

        [JsonProperty]
        public int CurrentQuantity { get; set; }

        [JsonConstructor]
        public ItemStockResponseDto(Guid productModelId, int currentQuantity) {
            this.CurrentQuantity = currentQuantity;
            this.ProductModelId = productModelId;
        }

    }
}
