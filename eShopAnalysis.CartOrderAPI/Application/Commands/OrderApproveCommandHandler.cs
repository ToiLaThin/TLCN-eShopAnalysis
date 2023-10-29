using eShopAnalysis.CartOrderAPI.Infrastructure;
using eShopAnalysis.CartOrderAPI.Services.BackchannelDto;
using MediatR;

namespace eShopAnalysis.CartOrderAPI.Application.Commands
{
    public class OrderApproveCommandHandler : IRequestHandler<OrderApproveCommand, IEnumerable<Guid>>
    {
        private IUnitOfWork _uOW;

        public OrderApproveCommandHandler(IUnitOfWork uOW)
        {
            _uOW = uOW;
        }
        public async Task<IEnumerable<Guid>> Handle(OrderApproveCommand request, CancellationToken cancellationToken)
        {
            var transaction = await _uOW.BeginTransactionAsync();
            var orderIdsToApprove = request.OrderIdsToApprove;
            foreach (var oId in orderIdsToApprove)
            {
                var orderToApprove = await _uOW.OrderRepository.GetOrder(oId);
                bool didSuccessfully = orderToApprove.ApproveOrder();
                if (didSuccessfully != true) {
                    _uOW.RollbackTransaction();
                    return null;
                }
            }
            await _uOW.CommitTransactionAsync(transaction);
            return orderIdsToApprove.ToList();


        }
    }
}
