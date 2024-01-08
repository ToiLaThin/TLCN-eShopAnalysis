using eShopAnalysis.EventBus.Abstraction;
using Newtonsoft.Json;

namespace eShopAnalysis.CouponSaleItemAPI.Application.IntegrationEvents
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

        [JsonProperty]
        public double? NewPriceOnSaleModel { get; }

        [JsonProperty]
        public Guid? OldSaleItemId { get; }

        [JsonProperty]
        public Guid? NewSaleItemId { get; }

        [JsonProperty]
        public string ProductName { get; }

        [JsonConstructor]
        public ProductModelPriceUpdatedIntegrationEvent(Guid oldProductId,
                                                        Guid newProductId,
                                                        Guid oldProductModelId,
                                                        Guid newProductModelId,
                                                        double oldPrice,
                                                        double newPrice,
                                                        double? newPriceOnSaleModel,
                                                        Guid? oldSaleItemId,
                                                        Guid? newSaleItemId,
                                                        string productName)
        {
            this.OldProductId = oldProductId;
            this.NewProductId = newProductId;
            this.OldProductModelId = oldProductModelId;
            this.NewProductModelId = newProductModelId;
            this.OldPrice = oldPrice;
            this.NewPrice = newPrice;
            this.NewPriceOnSaleModel = newPriceOnSaleModel;


            //neu model khong duoc sale thi hai SaleItemId Guid deu la null
            this.OldSaleItemId = oldSaleItemId;
            this.NewSaleItemId = newSaleItemId;

            //for toast to display the product name to user
            this.ProductName = productName;
        }
    }
}
