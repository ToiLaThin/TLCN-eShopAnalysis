using eShopAnalysis.PaymentAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace eShopAnalysis.PaymentAPI.Data
{
    public class PaymentContext: DbContext
    {
        public PaymentContext(DbContextOptions<PaymentContext> options) : base(options) { }

        public DbSet<UserCustomerMapping> UserCustomerMappings { get; set; }

        public DbSet<MomoTransaction> MomoTransactions { get; set; }

        public DbSet<StripeTransaction> StripeTransactions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(assembly: Assembly.GetExecutingAssembly());
        }
    }
}
