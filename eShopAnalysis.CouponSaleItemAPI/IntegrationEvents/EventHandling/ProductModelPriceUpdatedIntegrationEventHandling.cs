using eShopAnalysis.CouponSaleItemAPI.Application.IntegrationEvents;
using eShopAnalysis.CouponSaleItemAPI.Models;
using eShopAnalysis.CouponSaleItemAPI.Service;
using eShopAnalysis.EventBus.Abstraction;

namespace eShopAnalysis.CouponSaleItemAPI.IntegrationEvents
{
    //received from productCatalog Service when product model price changed
    public class ProductModelPriceUpdatedIntegrationEventHandling: IIntegrationEventHandler<ProductModelPriceUpdatedIntegrationEvent>
    {
        private readonly ISaleItemService _saleItemService;

        //handling when product model price changed, this create a new row of SaleItem point to new product instance and makr the old sale item ended
        public ProductModelPriceUpdatedIntegrationEventHandling(ISaleItemService saleItemService)
        {
            _saleItemService = saleItemService ?? throw new ArgumentNullException(nameof(saleItemService));
        }

        public async Task Handle(ProductModelPriceUpdatedIntegrationEvent @event)
        {
            //check if oldSaleItemId & newSaleItemId is null, if they 're return (means that the event handling does nothing if the product is not on sale now);
            if (@event.OldSaleItemId == null && @event.NewSaleItemId == null) {
                return;
            }
            //these guids not null for sure
            Guid oldSaleItemId = (Guid)@event.OldSaleItemId; 
            Guid newSaleItemId = (Guid)@event.NewSaleItemId;
            
            var serviceResult = await _saleItemService.CreateNewInstanceWhenProductModelPriceChanged(
                oldSaleItemId,
                newSaleItemId,
                @event.OldProductId,
                @event.OldProductModelId,
                @event.NewProductId,
                @event.NewProductModelId);
            return;
        }
    }
}
