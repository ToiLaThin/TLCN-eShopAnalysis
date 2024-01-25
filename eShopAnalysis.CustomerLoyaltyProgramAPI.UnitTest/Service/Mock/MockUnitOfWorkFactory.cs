using eShopAnalysis.CustomerLoyaltyProgramAPI.UnitOfWork;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.UnitTest.Service.Mock
{
    public class MockUnitOfWorkFactory
    {
        public static Mock<IUnitOfWork> GetUnitOfWorkMock()
        {
            Mock<IUnitOfWork> mockUoW = new Mock<IUnitOfWork>();
            mockUoW.Setup(m => m.HasActiveTransaction()).Returns(true);
            mockUoW.Setup(m => m.BeginTransactionAsync()).ReturnsAsync(new Mock<IDbContextTransaction>().Object);
            mockUoW.Setup(m => m.GetCurrentTransaction()).Returns(new Mock<IDbContextTransaction>().Object);
            mockUoW.Setup(m => m.CommitTransactionAsync(It.IsAny<IDbContextTransaction>())).Returns(Task.CompletedTask);
            mockUoW.Setup(m => m.RollbackTransaction()).Verifiable();
            mockUoW.Setup(m => m.RewardTransactionRepository).Returns(MockRepositoryFactory.GetRewardTransactionRepositoryMock().Object);
            mockUoW.Setup(m => m.UserRewardPointRepository).Returns(MockRepositoryFactory.GetUserRewardPointRepositoryMock().Object);


            return mockUoW;
        }
    }
}
