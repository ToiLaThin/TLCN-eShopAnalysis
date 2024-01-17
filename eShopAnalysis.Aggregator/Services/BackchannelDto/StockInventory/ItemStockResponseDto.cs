using Newtonsoft.Json;

namespace eShopAnalysis.Aggregator.Services.BackchannelDto
{
    //json prop is required for the model to be serialized or deserialized correctly
    //if not , there will be error

    //this will be response when decrease the stock item qty and also when approve order

    //this will be the response the stockInventory return when agregate read request for product info and & quatity
    //for the selected provider requirement on UI
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
