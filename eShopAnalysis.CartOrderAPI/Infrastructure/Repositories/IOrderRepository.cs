using eShopAnalysis.CartOrderAPI.Domain.DomainModels.OrderAggregate;

namespace eShopAnalysis.CartOrderAPI.Infrastructure.Repositories
{
    public interface IOrderRepository
    {
        Order? Add(Order order);

        Task<Order?> AddAsync(Order order);
        Task<Order> GetOrderAsyncWithChangeTracker(Guid orderId);
        void Update(Order order);
    }
}