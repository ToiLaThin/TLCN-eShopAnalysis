using Newtonsoft.Json;

namespace eShopAnalysis.Aggregator.Services.BackchannelDto
{
    //json prop is required for the model to be serialized or deserialized correctly
    //if not , there will be error

    /// <summary>
    /// response from StockInventory aggregate to get items 's stock in an order &  agregate read request for product info and & quantity for the selected provider requirement on UI
    /// response from StockInventory to aggregate to decrease stockItem
    /// in agregator read GetProductModelInfosWithStockOfProvider & in aggregator write ApproveOrdersAndModifyStocks
    /// </summary>
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
