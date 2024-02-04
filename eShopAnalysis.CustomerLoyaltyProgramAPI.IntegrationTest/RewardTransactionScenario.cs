using AutoMapper;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Dto;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Dto.BackchannelDto;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.IntegrationTest
{
    public class RewardTransactionScenario: BaseIntegrationTest
    {
        public RewardTransactionScenario(CustomerLoyaltyProgramWebAppFactory webAppFactory) : base(webAppFactory) {
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
            DbContext.UserRewardPoints.AddRange(DummyUserRewardPointData);
            DbContext.SaveChanges();
        }

        [Fact]
        public async Task GivenAExistingUserId_WhenGetRewardTransOfUser_ReturnOkResponse()
        {
            var client = CustomerLoyaltyProgramWebAppFactory.CreateClient();

            Guid existingUserId = base.DbContext.RewardTransactions.First().UserId;
            IEnumerable<RewardTransaction> rewardTransactionsOfUser = base.DbContext.RewardTransactions.Where(rt => rt.UserId.Equals(existingUserId));
            //IMapper mapper = _scope.ServiceProvider.GetRequiredService<IMapper>();
            //IEnumerable<RewardTransactionDto> rewardTransactionDtoOfUser = mapper.Map<IEnumerable<RewardTransaction>, IEnumerable<RewardTransactionDto>>(rewardTransactionsOfUser);
            HttpResponseMessage response = await client.GetAsync(IntegrationTestEndpoint.RewardTransaction.GetUserRewardTransaction + "?userId=" + existingUserId);
            IEnumerable<RewardTransactionDto> responseDto = JsonConvert.DeserializeObject<IEnumerable<RewardTransactionDto>>(await response.Content.ReadAsStringAsync());
            Assert.NotNull(response);

            responseDto.Should().BeAssignableTo<IEnumerable<RewardTransactionDto>>();
            //JsonConvert.SerializeObject(rewardTransactionsOfUser).Should().BeEquivalentTo(await response.Content.ReadAsStringAsync());
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GivenANonExistingUserId_WhenGetRewardTransOfUser_ReturnInvalidResponse()
        {
            var client = CustomerLoyaltyProgramWebAppFactory.CreateClient();

            Guid nonExistingUserId = Guid.Parse("5928ff97-e8f8-4cf4-bc9d-34cbf825c224");
            HttpResponseMessage response = await client.GetAsync(IntegrationTestEndpoint.RewardTransaction.GetUserRewardTransaction + "?userId=" + nonExistingUserId);
            IEnumerable<RewardTransactionDto> responseDto = JsonConvert.DeserializeObject<IEnumerable<RewardTransactionDto>>(await response.Content.ReadAsStringAsync());
            Assert.NotNull(response); //because it will throw exception
            response.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task GivenAddRewardTransForExistingUser_WhenAddRewardTransForCompleteOrdering_ReturnValidResponse()
        {
            var client = CustomerLoyaltyProgramWebAppFactory.CreateClient();

            Guid existingUserId = Guid.Parse("5928ff97-e8f8-4cf4-bc9d-34cbf825c054");
            RewardTransactionForCompleteOrderingAddRequestDto requestDto = new() { 
                UserId = existingUserId, 
                OrderPrice = 20000, 
                PointTransition = 50 
            };
            var httpContent = new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(IntegrationTestEndpoint.RewardTransaction.AddRewardTransactionForCompleteOrdering, httpContent);
            string responseStr = await response.Content.ReadAsStringAsync();
            BackChannelResponseDto<RewardTransactionDto> responseDto = JsonConvert.DeserializeObject<BackChannelResponseDto<RewardTransactionDto>>(responseStr);
            Assert.NotNull(response); //because it will throw exception
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK);
            responseDto.Should().BeAssignableTo<BackChannelResponseDto<RewardTransactionDto>>();
        }

        [Fact]
        public async Task GivenAddRewardTransForExistingUser_WhenAddRewardTransForApplyCoupon_ReturnResponse()
        {
            var client = CustomerLoyaltyProgramWebAppFactory.CreateClient();

            Guid existingUserId = Guid.Parse("5928ff97-e8f8-4cf4-bc9d-34cbf825c054");
            RewardTransactionForApplyCouponAddRequestDto requestDto = new()
            {
                UserId = existingUserId,
                DiscountType = CouponDiscountType.ByValue,
                DiscountValue = 20000,
                PointTransition = -20
            };
            var httpContent = new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(IntegrationTestEndpoint.RewardTransaction.AddRewardTransactionForApplyCoupon, httpContent);
            string responseStr = await response.Content.ReadAsStringAsync();
            BackChannelResponseDto<RewardTransactionDto> responseDto = JsonConvert.DeserializeObject<BackChannelResponseDto<RewardTransactionDto>>(responseStr);
            Assert.NotNull(response); //because it will throw exception
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK);
            responseDto.Should().BeAssignableTo<BackChannelResponseDto<RewardTransactionDto>>();
        }
    }
}
