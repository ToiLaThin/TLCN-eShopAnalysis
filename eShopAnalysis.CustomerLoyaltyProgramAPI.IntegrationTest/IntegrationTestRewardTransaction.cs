using eShopAnalysis.CustomerLoyaltyProgramAPI.Dto.BackchannelDto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.IntegrationTest
{
    public class IntegrationTestRewardTransaction: BaseIntegrationTest
    {
        public IntegrationTestRewardTransaction(CustomerLoyaltyProgramWebAppFactory webAppFactory) : base(webAppFactory) {
            this.SeedDb();
        }

        public override void SeedDb()
        {
            base.SeedDb();
            bool isDbJustCreated = DbContext.Database.EnsureCreated();
            if (DbContext.RewardTransactions.Any() == true)
            {
                return;
            }
            DbContext.RewardTransactions.AddRange(DummyRewardTranData);
            DbContext.SaveChanges();
        }

        [Fact]
        public async Task AddRewardTransactionForApplyCoupon()
        {
            var client = CustomerLoyaltyProgramWebAppFactory.CreateClient();
            RewardTransactionForApplyCouponAddRequestDto requestDto = new RewardTransactionForApplyCouponAddRequestDto()
            {
                UserId = Guid.Parse("a3532af6-5957-42f0-8585-ac630e1725c4"),
                DiscountType = Models.CouponDiscountType.ByValue,
                DiscountValue = 20000,
                PointTransition = -50
            };
            var content = new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("api/CustomerLoyaltyProgramAPI/RewardTransactionAPI/BackChannel/AddRewardTransactionForApplyCoupon", content);

            Assert.NotNull(response);
        }
    }
}
