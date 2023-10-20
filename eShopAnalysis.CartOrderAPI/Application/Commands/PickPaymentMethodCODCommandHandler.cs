using eShopAnalysis.CartOrderAPI.Application.Queries;
using eShopAnalysis.CartOrderAPI.Domain.DomainModels.OrderAggregate;
using eShopAnalysis.CartOrderAPI.Infrastructure;
using MediatR;

namespace eShopAnalysis.CartOrderAPI.Application.Commands
{
    public class PickPaymentMethodCODCommandHandler : IRequestHandler<PickPaymentMethodCODCommand, OrderViewModel>
    {
        IUnitOfWork _uOW;

        public PickPaymentMethodCODCommandHandler(IUnitOfWork uOW)
        {
            _uOW = uOW;
        }
        public async Task<OrderViewModel> Handle(PickPaymentMethodCODCommand request, CancellationToken cancellationToken)
        {
            var transaction = await _uOW.BeginTransactionAsync();
            Order orderToUpdate = await _uOW.OrderRepository.GetOrder(request.OrderId);
            bool didSuccessfully = orderToUpdate.PickPaymentMethodCOD();
            if (didSuccessfully == false) {
                _uOW.RollbackTransaction();
                return null;
            }
            await _uOW.CommitTransactionAsync(transaction);
            OrderViewModel orderReturn = new OrderViewModel() { OrderId = request.OrderId };
            return orderReturn;
        }
    }
}
