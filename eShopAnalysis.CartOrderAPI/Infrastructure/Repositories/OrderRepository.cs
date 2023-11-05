using eShopAnalysis.CartOrderAPI.Domain.DomainModels.OrderAggregate;
using Microsoft.EntityFrameworkCore;

namespace eShopAnalysis.CartOrderAPI.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private OrderCartContext _context;
        public OrderRepository(OrderCartContext context)
        {
            _context = context;
        }
        public Order? Add(Order order)
        {
            var orderAdded = _context.Orders.Add(order).Entity;
            //_context.SaveChanges();
            return orderAdded;
        }

        public async Task<Order?> AddAsync(Order order)
        {
            var orderAddedEntity = await _context.Orders.AddAsync(order);
            //_context.SaveChanges();
            return orderAddedEntity.Entity;
        }

        public async Task<Order> GetOrderAsyncWithChangeTracker(Guid orderId)
        {
            Order order = await _context.Orders.Include(c => c.Cart)
                                               .Include(c => c.Address)
                                               .FirstOrDefaultAsync(c => c.Id == orderId);
            if (order != null)
            {
                await _context.Entry(order)
                              .Reference(c => c.Cart)
                              .LoadAsync();
            }            
            return order;
        }

        public void Update(Order order)
        {
            _context.Entry(order).State = EntityState.Modified;
        }
    }
}
