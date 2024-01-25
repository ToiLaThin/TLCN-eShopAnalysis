using eShopAnalysis.CustomerLoyaltyProgramAPI.Dto;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Models;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Repository;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Service;
using eShopAnalysis.CustomerLoyaltyProgramAPI.UnitOfWork;
using eShopAnalysis.CustomerLoyaltyProgramAPI.UnitTest.Service.Mock;
using eShopAnalysis.CustomerLoyaltyProgramAPI.UnitTest.Utils;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Utilities.Factory;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.UnitTest.Service
{
    public class UnitTestRewardTransactionService
    {
        //AddRewardTransactionForCompleteOrdering is the same
        [Fact]
        public async Task GivenUserDoNotHaveRewardPointMapping_WhenAddRewardTransForApplyCoupon_ReturnServiceResponseFailed()
        {
            //Arrange
            //resetup mock user reward point repo for this test case
            Mock<IUserRewardPointRepository> mockUserRewardPointRepo = MockRepositoryFactory.GetUserRewardPointRepositoryMock();
            UserRewardPoint? nullRewardTrans = null; //to able to pass to ReturnAsync
            mockUserRewardPointRepo.Setup(m => m.GetAsync(It.IsAny<Guid>())).ReturnsAsync(nullRewardTrans);
            Mock<IUnitOfWork> mockUoW = MockUnitOfWorkFactory.GetUnitOfWorkMock();
            mockUoW.Setup(m => m.UserRewardPointRepository).Returns(mockUserRewardPointRepo.Object);

            RewardTransactionFactory rewardTransactionFactory = new RewardTransactionFactory();
            RewardTransactionService sut = new RewardTransactionService(mockUoW.Object, rewardTransactionFactory);

            //Act
            var result = await sut.AddRewardTransactionForApplyCoupon(Guid.NewGuid(), CouponDiscountType.ByPercent, 20, -50);


            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ServiceResponseDto<RewardTransaction>>();
            result.IsSuccess.Should().BeFalse();
            result.Should().Match<ServiceResponseDto<RewardTransaction>>(re => re.IsFailed == true && re.Error != null);
            result.Data.Should().BeNull();

        }

        [Fact]
        public async Task GivenInValidRewardTrandAndAddFailed_WhenAddRewardTransForApplyCoupon_ReturnServiceResponseFailed()
        {
            //Arrange
            //mock user reward point repo already got default get async set up
            Mock<IRewardTransactionRepository> mockRewardTransRepo = MockRepositoryFactory.GetRewardTransactionRepositoryMock();
            RewardTransaction? nullRewardTrans = null;
            mockRewardTransRepo.Setup(m => m.AddAsync(It.IsAny<RewardTransaction>())).ReturnsAsync(nullRewardTrans);
            Mock<IUnitOfWork> mockUoW = MockUnitOfWorkFactory.GetUnitOfWorkMock();
            mockUoW.Setup(m => m.RewardTransactionRepository).Returns(mockRewardTransRepo.Object);

            RewardTransactionFactory rewardTransactionFactory = new RewardTransactionFactory();
            RewardTransactionService sut = new RewardTransactionService(mockUoW.Object, rewardTransactionFactory);

            //Act
            var result = await sut.AddRewardTransactionForApplyCoupon(Guid.NewGuid(), CouponDiscountType.ByPercent, 20, -50);


            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ServiceResponseDto<RewardTransaction>>();
            result.IsSuccess.Should().BeFalse();
            result.Should().Match<ServiceResponseDto<RewardTransaction>>(re => re.IsFailed == true && re.Error != null);
            result.Data.Should().BeNull();

        }


        [Fact]
        public async Task GivenValidRewardTrandAndAddSuccessfully_WhenAddRewardTransForApplyCoupon_ReturnServiceResponseSuccess()
        {
            //Arrange
            Mock<IUnitOfWork> mockUoW = MockUnitOfWorkFactory.GetUnitOfWorkMock();

            RewardTransactionFactory rewardTransactionFactory = new RewardTransactionFactory();
            RewardTransactionService sut = new RewardTransactionService(mockUoW.Object, rewardTransactionFactory);

            //Act
            var result = await sut.AddRewardTransactionForApplyCoupon(Guid.NewGuid(), CouponDiscountType.ByPercent, 20, -50);


            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ServiceResponseDto<RewardTransaction>>();
            result.IsSuccess.Should().BeTrue();
            result.Should().Match<ServiceResponseDto<RewardTransaction>>(re => re.IsFailed == false && re.Error == null);
            result.Data.Should().NotBeNull();

        }

        [Fact]
        public async Task GivenNothingƯrong_WhenAddRewardTransForCompleteOrdering_ReturnServiceResponseStillSuccess()
        {
            //Arrange
            Mock<IUnitOfWork> mockUoW = MockUnitOfWorkFactory.GetUnitOfWorkMock();
            RewardTransactionFactory rewardTransactionFactory = new RewardTransactionFactory();
            RewardTransactionService sut = new RewardTransactionService(mockUoW.Object, rewardTransactionFactory);

            //Act
            var result = await sut.AddRewardTransactionForCompleteOrdering(Guid.NewGuid(), 20, 500);


            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ServiceResponseDto<RewardTransaction>>();
            result.IsSuccess.Should().BeTrue();
            result.Should().Match<ServiceResponseDto<RewardTransaction>>(re => re.IsFailed == false && re.Error == null);
            result.Data.Should().NotBeNull();

        }

        [Fact]
        public async Task GivenInValidRewardTrandAndAddFailed_WhenAddRewardTransForCompleteOrdering_ReturnServiceResponseFailed()
        {
            //Arrange
            //mock user reward point repo already got default get async set up
            Mock<IRewardTransactionRepository> mockRewardTransRepo = MockRepositoryFactory.GetRewardTransactionRepositoryMock();
            RewardTransaction? nullRewardTrans = null;
            mockRewardTransRepo.Setup(m => m.AddAsync(It.IsAny<RewardTransaction>())).ReturnsAsync(nullRewardTrans);
            Mock<IUnitOfWork> mockUoW = MockUnitOfWorkFactory.GetUnitOfWorkMock();
            mockUoW.Setup(m => m.RewardTransactionRepository).Returns(mockRewardTransRepo.Object);

            RewardTransactionFactory rewardTransactionFactory = new RewardTransactionFactory();
            RewardTransactionService sut = new RewardTransactionService(mockUoW.Object, rewardTransactionFactory);

            //Act
            var result = await sut.AddRewardTransactionForCompleteOrdering(Guid.NewGuid(), 20, 500);


            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ServiceResponseDto<RewardTransaction>>();
            result.IsSuccess.Should().BeFalse();
            result.Should().Match<ServiceResponseDto<RewardTransaction>>(re => re.IsFailed == true && re.Error != null);
            result.Data.Should().BeNull();

        }
    }
}
