using MediatR;
using eShopAnalysis.CartOrderAPI.Domain.DomainModels.DomainEvents;
using eShopAnalysis.CartOrderAPI.Infrastructure;
using eShopAnalysis.CartOrderAPI.Infrastructure.Repositories;
using eShopAnalysis.CartOrderAPI.Domain.DomainModels.OrderAggregate;

namespace eShopAnalysis.CartOrderAPI.Domain.DomainModels.DomainEventHandlers
{
    //TODO  fix all the business logic, may rename all these event name
    public class CartCheckoutRequestSentDomainEventHandler : INotificationHandler<CartCheckoutRequestSentDomainEvent>
    {
        private IOrderRepository _orderRepo; //do not inject UoW here because we do not anyone start an transaction or commit anything here
        public CartCheckoutRequestSentDomainEventHandler(IOrderRepository orderRepo)
        {
            _orderRepo = orderRepo;
        }

        public Task Handle(CartCheckoutRequestSentDomainEvent notification, CancellationToken cancellationToken)
        {
            var orderCreated = Order.CreateOrderFromCart(Guid.NewGuid(), Guid.NewGuid() ,notification.Cart);
            _orderRepo.Add(orderCreated);
            return Task.CompletedTask;

        }
    }
}
