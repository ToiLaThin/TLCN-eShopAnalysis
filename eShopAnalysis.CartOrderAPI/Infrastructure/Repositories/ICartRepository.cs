using eShopAnalysis.CartOrderAPI.Domain.DomainModels.CartAggregate;

namespace eShopAnalysis.CartOrderAPI.Infrastructure.Repositories
{
    public interface ICartRepository
    {
        CartSummary? Add(CartSummary cart);

        void Update(CartSummary cart);

        Task<CartSummary?> AddAsync(CartSummary cart);

        Task<CartSummary> GetCartAsyncWithChangeTracker(Guid cartId);
    }
}
