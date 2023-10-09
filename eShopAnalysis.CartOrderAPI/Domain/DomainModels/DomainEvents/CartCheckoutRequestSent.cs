using eShopAnalysis.CartOrderAPI.Domain.DomainModels.CartAggregate;
using eShopAnalysis.CartOrderAPI.Domain.SeedWork;

namespace eShopAnalysis.CartOrderAPI.Domain.DomainModels.DomainEvents
{
    public class CartCheckoutRequestSent : IDomainEvent
    {
        public CartSummary Cart { get; private set; }

        public CartCheckoutRequestSent(CartSummary cart)
        {
            Cart = cart;
        }

    }
}
