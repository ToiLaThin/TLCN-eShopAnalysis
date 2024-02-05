using eShopAnalysis.CustomerLoyaltyProgramAPI.Dto;
using eShopAnalysis.CustomerLoyaltyProgramAPI.IntegrationTest.Utils;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.IntegrationTest
{
    public class UserRewardPointScenario: BaseIntegrationTest
    {
        public UserRewardPointScenario(CustomerLoyaltyProgramWebAppFactory webAppFactory): base(webAppFactory) {
            this.SeedDb();
        }

        public override void SeedDb()
        {
            base.SeedDb();
            bool isDbJustCreated = base.DbContext.Database.EnsureCreated();
            if (DbContext.UserRewardPoints.Any() == true || DbContext.RewardTransactions.Any() == true) {
                return;
            }
            DbContext.RewardTransactions.AddRange(DummyRewardTranData);
            DbContext.UserRewardPoints.AddRange(DummyUserRewardPointData);
            DbContext.SaveChanges();
        }

        [Fact]
        public async Task GivenAnExistingUser_WhenGetUserRewardPoint_ReturnValidResponse()
        {
            //Arrange
            var client = CustomerLoyaltyProgramWebAppFactory.CreateClient();
            Guid existingUserId = DummyUserRewardPointData.First().UserId;
            HttpRequestMessage reqMessage = new HttpRequestMessage();
            reqMessage.Method = HttpMethod.Get;
            reqMessage.RequestUri = new Uri("http://localhost:/" + IntegrationTestEndpoint.UserRewardPoint.GetRewardPointOfUser);
            reqMessage.Headers.Add("userId", existingUserId.ToString());

            //Act
            HttpResponseMessage resp = await client.SendAsync(reqMessage);
            UserRewardPointDto responseDto = JsonConvert.DeserializeObject<UserRewardPointDto>(await resp.Content.ReadAsStringAsync());

            //Assert
            Assert.NotNull(resp);
            resp.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            responseDto.Should().BeAssignableTo<UserRewardPointDto>();
        }

        [Fact]
        public async Task GivenANonExistingUser_WhenGetUserRewardPoint_ReturnFailedResponse()
        {
            //Arrange
            var client = CustomerLoyaltyProgramWebAppFactory.CreateClient();
            Guid nonExistingUserId = Guid.Parse("8d05b772-f376-4a89-816b-b154c0befee6");
            HttpRequestMessage reqMessage = new HttpRequestMessage();
            reqMessage.Method = HttpMethod.Get;
            reqMessage.RequestUri = new Uri("http://localhost:/" + IntegrationTestEndpoint.UserRewardPoint.GetRewardPointOfUser);
            reqMessage.Headers.Add("userId", nonExistingUserId.ToString());

            //Act
            HttpResponseMessage resp = await client.SendAsync(reqMessage);
            string errorMsg = await resp.Content.ReadAsStringAsync();

            //Assert
            Assert.NotNull(resp);
            resp.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
            errorMsg.Should().BeAssignableTo<string>();

        }

        [Fact]
        public async Task GivenANonExistingUserId_WhenAddUserRewardPoint_ReturnSuccessResponse()
        {
            //Arrange
            var client = CustomerLoyaltyProgramWebAppFactory.CreateClient();
            Guid nonExistingUserId = Guid.Parse("8d05b772-f376-4a89-816b-b154c0befee6");
            HttpRequestMessage reqMessage = new HttpRequestMessage();
            reqMessage.Method = HttpMethod.Post;
            reqMessage.RequestUri = new Uri("http://localhost:/" + IntegrationTestEndpoint.UserRewardPoint.AddUserRewardInstance);
            reqMessage.Headers.Add("userId", nonExistingUserId.ToString());
            reqMessage.Headers.Add("initRewardPoint", "0");

            //Act
            HttpResponseMessage resp = await client.SendAsync(reqMessage);
            UserRewardPointDto respDto = JsonConvert.DeserializeObject<UserRewardPointDto>(await resp.Content.ReadAsStringAsync());

            //Assert
            Assert.NotNull(resp);
            resp.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            respDto.Should().BeAssignableTo<UserRewardPointDto>();

        }

        [Fact]
        public async Task GivenAnExistingUserId_WhenAddUserRewardPoint_ReturnFailedResponse()
        {
            //Arrange
            var client = CustomerLoyaltyProgramWebAppFactory.CreateClient();
            Guid existingUserId = DummyUserRewardPointData.First().UserId;
            HttpRequestMessage reqMessage = new HttpRequestMessage();
            reqMessage.Method = HttpMethod.Post;
            reqMessage.RequestUri = new Uri("http://localhost:/" + IntegrationTestEndpoint.UserRewardPoint.AddUserRewardInstance);
            reqMessage.Headers.Add("userId", existingUserId.ToString());

            //Act
            HttpResponseMessage resp = await client.SendAsync(reqMessage);
            string errorMsg = await resp.Content.ReadAsStringAsync();

            //Assert
            Assert.NotNull(resp);
            resp.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
            errorMsg.Should().BeAssignableTo<string>();

        }


    }
}
