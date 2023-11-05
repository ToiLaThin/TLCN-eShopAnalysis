using eShopAnalysis.CartOrderAPI.Application.Result;
using eShopAnalysis.CartOrderAPI.Domain.DomainModels.OrderAggregate;
using eShopAnalysis.CartOrderAPI.Infrastructure;
using eShopAnalysis.CartOrderAPI.Infrastructure.Repositories;
using MediatR;

namespace eShopAnalysis.CartOrderAPI.Application.Commands
{
    public class OrderInfoConfirmCommandHandler : IRequestHandler<OrderInfoConfirmCommand, CommandHandlerResponseDto<Order>>
    {
        private IUnitOfWork _uOW;
        public OrderInfoConfirmCommandHandler(IUnitOfWork uOW) {
            _uOW = uOW;
        }

        public async Task<CommandHandlerResponseDto<Order>> Handle(OrderInfoConfirmCommand request, CancellationToken cancellationToken)
        {
            var transaction = await _uOW.BeginTransactionAsync();
            //retrieve the order we want to update
            var orderToUpdateInfo = await _uOW.OrderRepository.GetOrderAsyncWithChangeTracker(request.OrderId);
            //call the business logic to update customer info
            bool didSuccessfully = orderToUpdateInfo.UpdateCustomerInfoOnThisOrder(phoneNumber: request.PhoneNumber, address: request.Address);
            if (didSuccessfully != true) {
                _uOW.RollbackTransaction();
                return CommandHandlerResponseDto<Order>.Failure("Cannot update customer info on this order");
            }

            //save it and send any kind of integration event
            await _uOW.CommitTransactionAsync(transaction);

            //return result which is the updated order;
            var result = await _uOW.OrderRepository.GetOrderAsyncWithChangeTracker(orderId: request.OrderId);
            return CommandHandlerResponseDto<Order>.Success(result);

        }
    }
}
