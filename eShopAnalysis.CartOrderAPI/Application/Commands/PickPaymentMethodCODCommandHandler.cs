using eShopAnalysis.CartOrderAPI.Application.Queries;
using eShopAnalysis.CartOrderAPI.Application.Result;
using eShopAnalysis.CartOrderAPI.Domain.DomainModels.OrderAggregate;
using eShopAnalysis.CartOrderAPI.Infrastructure;
using MediatR;

namespace eShopAnalysis.CartOrderAPI.Application.Commands
{
    public class PickPaymentMethodCODCommandHandler : IRequestHandler<PickPaymentMethodCODCommand, CommandHandlerResponseDto<Order>>
    {
        IUnitOfWork _uOW;

        public PickPaymentMethodCODCommandHandler(IUnitOfWork uOW)
        {
            _uOW = uOW;
        }
        public async Task<CommandHandlerResponseDto<Order>> Handle(PickPaymentMethodCODCommand request, CancellationToken cancellationToken)
        {
            var transaction = await _uOW.BeginTransactionAsync();
            Order orderToUpdate = await _uOW.OrderRepository.GetOrderAsyncWithChangeTracker(request.OrderId);
            bool didSuccessfully = orderToUpdate.PickPaymentMethodCOD();
            if (didSuccessfully == false) {
                _uOW.RollbackTransaction();
                return CommandHandlerResponseDto<Order>.Failure("cannot pick payment method COD for this order");
            }
            await _uOW.CommitTransactionAsync(transaction);
            //return result which is the updated order;
            var orderReturn = await _uOW.OrderRepository.GetOrderAsyncWithChangeTracker(orderId: request.OrderId);
            return CommandHandlerResponseDto<Order>.Success(orderReturn);
        }
    }
}
