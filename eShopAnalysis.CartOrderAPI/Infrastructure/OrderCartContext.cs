using eShopAnalysis.CartOrderAPI.Domain.DomainModels.CartAggregate;
using eShopAnalysis.CartOrderAPI.Domain.DomainModels.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace eShopAnalysis.CartOrderAPI.Infrastructure
{
    public class OrderCartContext: DbContext
    {
        public DbSet<CartSummary> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        public DbSet<Order> Orders { get; set; }
        public OrderCartContext(DbContextOptions<OrderCartContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(assembly: Assembly.GetExecutingAssembly());
        }
    }
}
