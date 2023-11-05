using eShopAnalysis.CartOrderAPI.Application.IntegrationEvents.Event;
using eShopAnalysis.CartOrderAPI.Application.Result;
using eShopAnalysis.CartOrderAPI.Domain.DomainModels.OrderAggregate;
using eShopAnalysis.CartOrderAPI.Infrastructure;
using eShopAnalysis.EventBus.Abstraction;
using MediatR;

namespace eShopAnalysis.CartOrderAPI.Application.Commands
{
    public class SetOrderCheckoutedOnlineCommandHandler : IRequestHandler<SetOrderCheckoutedOnlineCommand, CommandHandlerResponseDto<Order>>
    {
        private IUnitOfWork _uOW;
        IEventBus _eventBus;
        public SetOrderCheckoutedOnlineCommandHandler(IUnitOfWork uOW, IEventBus eventBus)
        {
            _uOW = uOW;
            _eventBus = eventBus;
        }

        public async Task<CommandHandlerResponseDto<Order>> Handle(SetOrderCheckoutedOnlineCommand request, CancellationToken cancellationToken)
        {
            var transaction = await _uOW.BeginTransactionAsync();
            Order orderToUpdate = await _uOW.OrderRepository.GetOrderAsyncWithChangeTracker(request.OrderId);
            bool didSuccessfully = orderToUpdate.SetAsCheckoutedOnlineByMethod(request.PaymentMethod, request.DateCheckouted);
            if (didSuccessfully == false) {
                _uOW.RollbackTransaction();
                return CommandHandlerResponseDto<Order>.Failure("Cannot set as checkouted online");
            }
            await _uOW.CommitTransactionAsync(transaction);
            Order orderReturn = await _uOW.OrderRepository.GetOrderAsyncWithChangeTracker(request.OrderId);
            _eventBus.Publish(new OrderStatusChangedToCheckoutedIntegrationEvent(
                orderId: request.OrderId,
                userId: orderReturn.Cart.UserId,
                paidAmount: orderReturn.Cart.TotalPriceFinal,
                paymentMethod: request.PaymentMethod,
                dateCheckouted: request.DateCheckouted)
            );
            return CommandHandlerResponseDto<Order>.Success(orderReturn);
        }
    }
}
