using eShopAnalysis.CartOrderAPI.Application.IntegrationEvents.Event;
using eShopAnalysis.CartOrderAPI.Domain.DomainModels.OrderAggregate;
using eShopAnalysis.CartOrderAPI.Infrastructure;
using eShopAnalysis.EventBus.Abstraction;
using MediatR;

namespace eShopAnalysis.CartOrderAPI.Application.Commands
{
    public class SetOrderCheckoutedOnlineCommandHandler : IRequestHandler<SetOrderCheckoutedOnlineCommand, Order>
    {
        private IUnitOfWork _uOW;
        IEventBus _eventBus;
        public SetOrderCheckoutedOnlineCommandHandler(IUnitOfWork uOW, IEventBus eventBus)
        {
            _uOW = uOW;
            _eventBus = eventBus;
        }

        public async Task<Order> Handle(SetOrderCheckoutedOnlineCommand request, CancellationToken cancellationToken)
        {
            var transaction = await _uOW.BeginTransactionAsync();
            Order orderToUpdate = await _uOW.OrderRepository.GetOrder(request.OrderId);
            bool didSuccessfully = orderToUpdate.SetAsCheckoutedOnlineByMethod(request.PaymentMethod, request.DateCheckouted);
            if (didSuccessfully == false)
            {
                _uOW.RollbackTransaction();
                return null;
            }
            await _uOW.CommitTransactionAsync(transaction);
            Order orderReturn = await _uOW.OrderRepository.GetOrder(request.OrderId);
            _eventBus.Publish(new OrderStatusChangedToCheckoutedIntegrationEvent(
                orderId: request.OrderId,
                userId: orderReturn.Cart.UserId,
                paidAmount: orderReturn.Cart.TotalPriceFinal,
                paymentMethod: request.PaymentMethod,
                dateCheckouted: request.DateCheckouted)
            );
            return orderReturn;
        }
    }
}
