using eShopAnalysis.CartOrderAPI.Domain.DomainModels.OrderAggregate;

namespace eShopAnalysis.CartOrderAPI.Infrastructure.Repositories
{
    public interface IOrderRepository
    {
        Order? Add(Order order);
        Task<Order> GetOrder(Guid orderId);
        void Update(Order order);
    }
}