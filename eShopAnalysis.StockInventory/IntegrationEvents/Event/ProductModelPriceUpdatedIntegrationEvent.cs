using eShopAnalysis.EventBus.Abstraction;
using Newtonsoft.Json;

namespace eShopAnalysis.StockInventoryAPI.IntegrationEvents
{
    //publish to saleItem, stockInventory (to modify the ProductId and ProductModelId)
    //publish to notificationHub to adjust cart on localStorage
    public record ProductModelPriceUpdatedIntegrationEvent : IntegrationEvent
    {
        [JsonProperty]
        public Guid OldProductId { get; }

        [JsonProperty]
        public Guid NewProductId { get; }

        [JsonProperty]
        public Guid OldProductModelId { get; }

        [JsonProperty]
        public Guid NewProductModelId { get; }

        [JsonProperty]
        public double OldPrice { get; }

        [JsonProperty]
        public double NewPrice { get; }

        [JsonConstructor]
        public ProductModelPriceUpdatedIntegrationEvent(Guid oldProductId,
                                                        Guid newProductId,
                                                        Guid oldProductModelId,
                                                        Guid newProductModelId,
                                                        double oldPrice,
                                                        double newPrice)
        {
            this.OldProductId = oldProductId;
            this.NewProductId = newProductId;
            this.OldProductModelId = oldProductModelId;
            this.NewProductModelId = newProductModelId;
            this.OldPrice = oldPrice;
            this.NewPrice = newPrice;
        }
    }
}
