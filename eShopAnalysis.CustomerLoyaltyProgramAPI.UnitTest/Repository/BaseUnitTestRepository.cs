using eShopAnalysis.CustomerLoyaltyProgramAPI.Data;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Models;
using eShopAnalysis.CustomerLoyaltyProgramAPI.UnitTest.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.UnitTest.Repository
{
    /// <summary>
    /// please seed db in the derived class, because each test in the test class run , the base constructor & dispose will be call. Just put break point, run test in debug mode to see, every test run, these init & dispose will call over and over
    /// </summary>
    public class BaseUnitTestRepository: IDisposable
    {
        private string connectionString = "Host=localhost;Username=postgres;Password=123;Database=CustomerLoyaltyProgramUnitTestDb";
        protected PostgresDbContext PostgresDbContext { get; set; }

        protected IEnumerable<RewardTransaction> DummyRewardTranData { get; set; }

        protected IEnumerable<UserRewardPoint> DummyUserRewardPointData { get; set; }


        public BaseUnitTestRepository()
        {
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
