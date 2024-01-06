using eShopAnalysis.EventBus.Abstraction;
using eShopAnalysis.StockInventoryAPI.Services;

namespace eShopAnalysis.StockInventoryAPI.IntegrationEvents
{
    //received from productCatalog Service when product model price changed
    public class ProductModelPriceUpdatedIntegrationEventHandling : IIntegrationEventHandler<ProductModelPriceUpdatedIntegrationEvent>
    {
        private readonly IStockInventoryService _stockInventoryService;

        public ProductModelPriceUpdatedIntegrationEventHandling(IStockInventoryService stockInventoryService)
        {
            _stockInventoryService = stockInventoryService;
        }

        public async Task Handle(ProductModelPriceUpdatedIntegrationEvent @event)
        {
            var serviceResult = await _stockInventoryService.UpdateIdsAfterProductModelPriceChanged(oldProductId: @event.OldProductId,
                                                                                              newProductId: @event.NewProductId,
                                                                                              oldProductModelId: @event.OldProductModelId,
                                                                                              newProductModelId: @event.NewProductModelId);
            if (serviceResult.IsFailed) {
                throw new Exception(serviceResult.Error);
            }

            var data = serviceResult.Data; //debug only
            return;
        }
    }
}
