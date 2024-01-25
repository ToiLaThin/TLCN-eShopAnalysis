using eShopAnalysis.CustomerLoyaltyProgramAPI.Data;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Models;
using eShopAnalysis.CustomerLoyaltyProgramAPI.UnitTest.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.UnitTest.UnitOfWork
{
    public class BaseUnitTestUnitOfWork: IDisposable
    {
        private string connectionString = "Host=localhost;Username=postgres;Password=123;Database=CustomerLoyaltyProgramUnitTestDb";
        protected PostgresDbContext PostgresDbContext { get; set; }

        protected IEnumerable<RewardTransaction> DummyRewardTranData { get; set; }

        protected IEnumerable<UserRewardPoint> DummyUserRewardPointData { get; set; }
        public BaseUnitTestUnitOfWork() {
            DbContextOptions<PostgresDbContext> dbContextOption = new DbContextOptionsBuilder<PostgresDbContext>().UseNpgsql(connectionString: this.connectionString).Options;
            PostgresDbContext = new PostgresDbContext(dbContextOption);
            DummyRewardTranData = DummyDataProvider.GetRewardTransactionDummyData();
            DummyUserRewardPointData = DummyDataProvider.GetUserRewardPointDummyData();
        }

        //Determine how db data is seed in derived class
        public virtual void SeedDb()
        {

        }


        public void Dispose()
        {
            PostgresDbContext.Database.EnsureDeleted();
        }
    }
}
