using eShopAnalysis.CartOrderAPI.Domain.DomainModels.CartAggregate;
using eShopAnalysis.CartOrderAPI.Domain.SeedWork;

namespace eShopAnalysis.CartOrderAPI.Domain.DomainModels.DomainEvents
{
    public class CartCheckoutRequestSentDomainEvent : IDomainEvent
    {
        public CartSummary Cart { get; private set; }

        public CartCheckoutRequestSentDomainEvent(CartSummary cart)
        {
            Cart = cart;
        }

    }
}
