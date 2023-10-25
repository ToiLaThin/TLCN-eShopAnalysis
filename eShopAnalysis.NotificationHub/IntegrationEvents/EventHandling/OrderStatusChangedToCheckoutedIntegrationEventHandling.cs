using eShopAnalysis.CartOrderAPI.Application.IntegrationEvents.Event;
using eShopAnalysis.EventBus.Abstraction;
using Microsoft.AspNetCore.SignalR;

namespace eShopAnalysis.NotificationHub.IntegrationEvents.EventHandling
{
    public class OrderStatusChangedToCheckoutedIntegrationEventHandling : IIntegrationEventHandler<OrderStatusChangedToCheckoutedIntegrationEvent>
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        public OrderStatusChangedToCheckoutedIntegrationEventHandling(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task Handle(OrderStatusChangedToCheckoutedIntegrationEvent @event)
        {
            await _hubContext.Clients.Group(@event.UserId.ToString())
                               .SendAsync(EventsToSentClient.OrderStatusChanged, new
                               {
                                   OrderId = @event.OrderId,
                                   DateCheckouted = @event.DateCheckouted,
                                   PaymentMethod = @event.PaymentMethod,
                                   PaidAmount = @event.PaidAmount,
                               });
        }
    }
}
