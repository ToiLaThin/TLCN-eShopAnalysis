using eShopAnalysis.CustomerLoyaltyProgramAPI.Models;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Repository;
using eShopAnalysis.CustomerLoyaltyProgramAPI.UnitTest.Utils;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.UnitTest.Service.Mock
{
    public class MockRepositoryFactory
    {
        /// <summary>
        /// if the test method require different return result from pre set up method, override them with new setup
        /// </summary>
        /// <returns>mock of interface with pre set up method</returns>
        public static Mock<IRewardTransactionRepository> GetRewardTransactionRepositoryMock()
        {
            Mock<IRewardTransactionRepository> mockRepo = new Mock<IRewardTransactionRepository>();
            var dummyRewardTransData = DummyDataProvider.GetRewardTransactionDummyData();
            mockRepo.Setup(m => m.GetAsync(It.IsAny<Guid>())).ReturnsAsync(dummyRewardTransData.First());
            mockRepo.Setup(m => m.GetAsQueryable()).Returns(dummyRewardTransData.AsQueryable());
            mockRepo.Setup(m => m.Get(It.IsAny<Guid>())).Returns<RewardTransaction>((id) => dummyRewardTransData.First());
            mockRepo.Setup(m => m.Update(It.IsAny<RewardTransaction>())).Returns<RewardTransaction>((id) => dummyRewardTransData.First());
            mockRepo.Setup(m => m.Add(It.IsAny<RewardTransaction>())).Returns<RewardTransaction>((id) => dummyRewardTransData.First());
            mockRepo.Setup(m => m.AddAsync(It.IsAny<RewardTransaction>())).ReturnsAsync(dummyRewardTransData.First());
            mockRepo.Setup(m => m.Delete(It.IsAny<RewardTransaction>())).Returns<RewardTransaction>((id) => dummyRewardTransData.First());

            return mockRepo;
        }

        /// <summary>
        /// if the test method require different return result from pre set up method, override them with new setup
        /// </summary>
        /// <returns>mock of interface with pre set up method</returns>
        public static Mock<IUserRewardPointRepository> GetUserRewardPointRepositoryMock()
        {
            Mock<IUserRewardPointRepository> mockRepo = new Mock<IUserRewardPointRepository>();
            var dummyUserRewardPointData = DummyDataProvider.GetUserRewardPointDummyData();
            mockRepo.Setup(m => m.GetAsQueryable()).Returns(dummyUserRewardPointData.AsQueryable());
            mockRepo.Setup(m => m.GetAsync(It.IsAny<Guid>())).ReturnsAsync(dummyUserRewardPointData.First());
            mockRepo.Setup(m => m.Get(It.IsAny<Guid>())).Returns<RewardTransaction>((id) => dummyUserRewardPointData.First());
            mockRepo.Setup(m => m.Add(It.IsAny<UserRewardPoint>())).Returns<UserRewardPoint>((id) => dummyUserRewardPointData.First());
            mockRepo.Setup(m => m.AddAsync(It.IsAny<UserRewardPoint>())).ReturnsAsync(dummyUserRewardPointData.First());
            mockRepo.Setup(m => m.Update(It.IsAny<UserRewardPoint>())).Returns<UserRewardPoint>((id) => dummyUserRewardPointData.First());
            mockRepo.Setup(m => m.Delete(It.IsAny<UserRewardPoint>())).Returns<UserRewardPoint>((id) => dummyUserRewardPointData.First());

            return mockRepo;
        }
    }
}
