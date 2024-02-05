using eShopAnalysis.CustomerLoyaltyProgramAPI.Data;
using eShopAnalysis.CustomerLoyaltyProgramAPI.IntegrationTest.Utils;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.IntegrationTest
{
    public class BaseIntegrationTest : IClassFixture<CustomerLoyaltyProgramWebAppFactory>, IDisposable
    {
        protected readonly IServiceScope _scope;
        protected PostgresDbContext DbContext { get; private set; }

        protected IEnumerable<RewardTransaction> DummyRewardTranData { get; set; }

        protected IEnumerable<UserRewardPoint> DummyUserRewardPointData { get; set; }

        protected CustomerLoyaltyProgramWebAppFactory CustomerLoyaltyProgramWebAppFactory { get; private set; }

        protected BaseIntegrationTest(CustomerLoyaltyProgramWebAppFactory webAppFactory) {
            var scope = webAppFactory.Services.CreateScope();
            DbContext = scope.ServiceProvider.GetRequiredService<PostgresDbContext>();
            DummyRewardTranData = DummyDataProvider.GetRewardTransactionDummyData();
            DummyUserRewardPointData = DummyDataProvider.GetUserRewardPointDummyData();
            CustomerLoyaltyProgramWebAppFactory = webAppFactory;
        }

        public virtual void SeedDb()
        {

        }

        public void Dispose()
        {
            _scope?.Dispose();
            DbContext.Dispose();
        }        
    }
}
