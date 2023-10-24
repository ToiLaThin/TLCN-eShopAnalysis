using eShopAnalysis.CartOrderAPI.Application.Queries;
using eShopAnalysis.CartOrderAPI.Domain.DomainModels.OrderAggregate;
using eShopAnalysis.CartOrderAPI.Infrastructure;
using MediatR;

namespace eShopAnalysis.CartOrderAPI.Application.Commands
{
    public class SetOrderCheckoutedOnlineCommandHandler : IRequestHandler<SetOrderCheckoutedOnlineCommand, Order>
    {
        private IUnitOfWork _uOW;
        public SetOrderCheckoutedOnlineCommandHandler(IUnitOfWork uOW)
        {
            _uOW = uOW;
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
            return orderReturn;
        }
    }
}
