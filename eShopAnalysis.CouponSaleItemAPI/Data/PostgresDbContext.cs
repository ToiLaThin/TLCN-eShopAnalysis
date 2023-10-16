using eShopAnalysis.CouponSaleItemAPI.Models;
using eShopAnalysis.CouponSaleItemAPI.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection;

namespace eShopAnalysis.CouponSaleItemAPI.Data
{
    public class PostgresDbContext : DbContext
    {
        public PostgresDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }
        public DbSet<SaleItem> SaleItems { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<CouponUser> CouponUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(assembly: Assembly.GetExecutingAssembly());
        }


    }
}
