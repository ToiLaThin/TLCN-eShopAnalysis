using eShopAnalysis.CustomerLoyaltyProgramAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.Data
{
    public class PostgresDbContext: DbContext
    {
        public PostgresDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }

        public DbSet<UserRewardPoint> UserRewardPoints { get; set; }

        public DbSet<RewardTransaction> RewardTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(assembly: Assembly.GetExecutingAssembly());
        }


    }
}
