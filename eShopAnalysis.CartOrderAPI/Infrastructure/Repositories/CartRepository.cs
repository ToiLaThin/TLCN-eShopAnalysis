using eShopAnalysis.CartOrderAPI.Domain.DomainModels.CartAggregate;
using Microsoft.EntityFrameworkCore;

namespace eShopAnalysis.CartOrderAPI.Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly OrderCartContext _context;
        public CartRepository(OrderCartContext context) { 
            _context = context;
        }
        public CartSummary? Add(CartSummary cart)
        {
            var cartAdded = _context.Carts.Add(cart).Entity;
            //_context.SaveChanges();
            return cartAdded;
        }

        public async Task<CartSummary> GetCart(Guid cartId)
        {
            CartSummary cart = await _context.Carts.Include(c => c.Items)
                                                   .FirstOrDefaultAsync(c => c.Id == cartId);
            if (cart != null) {
                await _context.Entry(cart)
                              .Collection(c => c.Items)
                              .LoadAsync();
            }

            return cart;
        }

        public void Update(CartSummary cart)
        {
            _context.Entry(cart).State = EntityState.Modified;
        }
    }
}
