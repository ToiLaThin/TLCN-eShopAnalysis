using eShopAnalysis.CartOrderAPI.Application.Commands;
using eShopAnalysis.CartOrderAPI.IntegrationEvents;
using eShopAnalysis.EventBus.Abstraction;
using MediatR;

namespace eShopAnalysis.CartOrderAPI.Application.IntegrationEvents.EventHandling
{
    public class OrderPaymentTransactionCompletedIntegrationEventHandling : IIntegrationEventHandler<OrderPaymentTransactionCompletedIntegrationEvent>
    {
        private IMediator _mediator;

        public OrderPaymentTransactionCompletedIntegrationEventHandling(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(OrderPaymentTransactionCompletedIntegrationEvent @event)
        {
            var cmd = new SetOrderCheckoutedOnlineCommand(
                    orderId: @event.OrderId, 
                    paymentMethod: @event.PaymentMethod, 
                    dateCheckouted: @event.DateCheckouted
                    );
            await _mediator.Send(cmd);
            
        }
    }
}
