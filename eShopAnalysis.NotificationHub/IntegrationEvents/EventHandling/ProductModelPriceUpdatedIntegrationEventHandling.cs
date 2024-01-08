using eShopAnalysis.EventBus.Abstraction;
using eShopAnalysis.NotificationHub.Application.IntegrationEvents.Event;
using Microsoft.AspNetCore.SignalR;

namespace eShopAnalysis.NotificationHub.IntegrationEvents.EventHandling
{
    //received from productCatalog service when product model price changed to notify user having this product model in cart
    public class ProductModelPriceUpdatedIntegrationEventHandling : IIntegrationEventHandler<ProductModelPriceUpdatedIntegrationEvent>
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        public ProductModelPriceUpdatedIntegrationEventHandling(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }
        
        public async Task Handle(ProductModelPriceUpdatedIntegrationEvent @event)
        {
            await _hubContext.Clients.All.SendAsync(EventsToSentClient.ProductModelPriceChanged, new
            {
                OldProductId = @event.OldProductId,
                NewProductId = @event.NewProductId,
                OldProductModelId = @event.OldProductModelId,
                NewProductModelId = @event.NewProductModelId,
                NewPrice = @event.NewPrice,
                NewPriceOnSaleModel = @event.NewPriceOnSaleModel,
                ProductName = @event.ProductName
            });
        }
    }
}
