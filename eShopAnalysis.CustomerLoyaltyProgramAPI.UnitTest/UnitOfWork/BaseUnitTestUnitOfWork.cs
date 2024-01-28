using eShopAnalysis.CustomerLoyaltyProgramAPI.Data;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Models;
using eShopAnalysis.CustomerLoyaltyProgramAPI.UnitTest.Repository;
using eShopAnalysis.CustomerLoyaltyProgramAPI.UnitTest.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.UnitTest.UnitOfWork
{
    public class BaseUnitTestUnitOfWork: IDisposable, IClassFixture<FixtureUnitTestUnitOfWork>
    {
        //private string connectionString = "Host=localhost;Username=postgres;Password=123;Database=CustomerLoyaltyProgramUnitTestDb";
        private string connectionString = String.Empty;
        protected PostgresDbContext PostgresDbContext { get; set; }

        protected IEnumerable<RewardTransaction> DummyRewardTranData { get; set; }

        protected IEnumerable<UserRewardPoint> DummyUserRewardPointData { get; set; }
        public BaseUnitTestUnitOfWork(FixtureUnitTestUnitOfWork fixtureUnitTestUnitOfWork) {
            this.connectionString = fixtureUnitTestUnitOfWork.ConnectionString;
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
            PostgresDbContext.RewardTransactions.RemoveRange(PostgresDbContext.RewardTransactions.ToList());
            PostgresDbContext.UserRewardPoints.RemoveRange(PostgresDbContext.UserRewardPoints.ToList());
            PostgresDbContext.SaveChanges();
        }
    }
}
