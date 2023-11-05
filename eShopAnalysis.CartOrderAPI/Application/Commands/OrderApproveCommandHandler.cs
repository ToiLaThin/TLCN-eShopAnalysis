using eShopAnalysis.CartOrderAPI.Application.Result;
using eShopAnalysis.CartOrderAPI.Infrastructure;
using eShopAnalysis.CartOrderAPI.Services.BackchannelDto;
using MediatR;

namespace eShopAnalysis.CartOrderAPI.Application.Commands
{
    public class OrderApproveCommandHandler : IRequestHandler<OrderApproveCommand, CommandHandlerResponseDto<IEnumerable<Guid>>>
    {
        private IUnitOfWork _uOW;

        public OrderApproveCommandHandler(IUnitOfWork uOW)
        {
            _uOW = uOW;
        }
        public async Task<CommandHandlerResponseDto<IEnumerable<Guid>>> Handle(OrderApproveCommand request, CancellationToken cancellationToken)
        {
            var transaction = await _uOW.BeginTransactionAsync();
            var orderIdsToApprove = request.OrderIdsToApprove;
            foreach (var oId in orderIdsToApprove)
            {
                var orderToApprove = await _uOW.OrderRepository.GetOrderAsyncWithChangeTracker(oId);
                bool didSuccessfully = orderToApprove.ApproveOrder(); //update
                if (didSuccessfully != true) {
                    _uOW.RollbackTransaction();
                    return CommandHandlerResponseDto<IEnumerable<Guid>>.Failure("One of the order cannot be approved");
                }
            }
            await _uOW.CommitTransactionAsync(transaction);
            return CommandHandlerResponseDto<IEnumerable<Guid>>.Success(orderIdsToApprove.ToList());


        }
    }
}
