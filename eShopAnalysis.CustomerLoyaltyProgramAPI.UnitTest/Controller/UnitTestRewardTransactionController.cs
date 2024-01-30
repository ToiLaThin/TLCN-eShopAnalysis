using AutoMapper;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Controllers;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Dto;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Dto.BackchannelDto;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Models;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Service;
using eShopAnalysis.CustomerLoyaltyProgramAPI.UnitTest.Utils;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.UnitTest.Controller
{
    public class UnitTestRewardTransactionController
    {
        [Fact]
        public async Task WhenAddRewardTransForApplyCouponSuccess_ReturnSuccessBackChannelResult()
        {
            //Arrange all dependencies & used method
            Mock<IRewardTransactionService> mockService = new Mock<IRewardTransactionService>();
            Mock<IMapper> mockMapper = new Mock<IMapper>();
            RewardTransactionDto returnRewardTransDto = new RewardTransactionDto()
            {
                UserId = Guid.Parse("93d3b6d6-0187-4a87-be75-57f16e9eb253"),
                DateTransition = new DateTime(2024, 12, 30, 2, 2, 2, DateTimeKind.Utc),
                PointAfterTransaction = 20,
                PointBeforeTransaction = 25,
                PointTransition = -5,
                RewardTransactionId = Guid.Parse("c574afbc-3285-4de6-8230-3970605e176f"),
                Origin = new OriginJson()
                {
                    Reason = Reason.ApplyCoupon,
                    DiscountType = CouponDiscountType.ByValue,
                    DiscountValue = 2000
                }
            };
            ServiceResponseDto<RewardTransaction> returnSuccessServiceResponse = ServiceResponseDto<RewardTransaction>.Success(new RewardTransaction()
            {
                UserId = Guid.Parse("93d3b6d6-0187-4a87-be75-57f16e9eb253"),
                DateTransition = new DateTime(2024, 12, 30, 2, 2, 2, DateTimeKind.Utc),
                PointAfterTransaction = 20,
                PointBeforeTransaction = 25,
                PointTransition = -5,
                RewardTransactionId = Guid.Parse("c574afbc-3285-4de6-8230-3970605e176f"),
                Origin = new OriginJson()
                {
                    Reason = Reason.ApplyCoupon,
                    DiscountType = CouponDiscountType.ByValue,
                    DiscountValue = 2000
                }
            });
            mockService.Setup(m => m.AddRewardTransactionForApplyCoupon(It.IsAny<Guid>(), It.IsAny<CouponDiscountType>(), It.IsAny<double>(), It.IsAny<int>())).ReturnsAsync(returnSuccessServiceResponse);
            //must pass in params like this
            mockMapper.Setup(m => m.Map<RewardTransactionDto>(It.IsAny<RewardTransaction>())).Returns(returnRewardTransDto);
            RewardTransactionForApplyCouponAddRequestDto requestDto = new()
            {
                UserId = Guid.NewGuid(),
                DiscountType = Models.CouponDiscountType.ByValue,
                DiscountValue = 200,
                PointTransition = -20
            };
            RewardTransactionController controller = new RewardTransactionController(mockService.Object, mockMapper.Object);

            //Act
            var backChannelResult = await controller.AddRewardTransactionForApplyCoupon(requestDto);

            //Assert
            Assert.NotNull(backChannelResult);
            backChannelResult.Should().BeOfType<BackChannelResponseDto<RewardTransactionDto>>();
            backChannelResult.IsSuccess.Should().BeTrue();
            backChannelResult.Result.Should().BeOneOf(ResultType.Success);
            backChannelResult.Data.Should().NotBeNull();
            backChannelResult.Data.Should().BeAssignableTo<RewardTransactionDto>();
            backChannelResult.Data.Should().BeEquivalentTo(returnRewardTransDto);

        }

        [Fact]
        public async Task WhenAddRewardTransForApplyCouponFailure_ReturnFailedBackChannelResult()
        {
            //Arrange all dependencies & used method
            Mock<IRewardTransactionService> mockService = new Mock<IRewardTransactionService>();
            Mock<IMapper> mockMapper = new Mock<IMapper>();
            ServiceResponseDto<RewardTransaction> returnFailedServiceResponse = ServiceResponseDto<RewardTransaction>.Failure("any message");
            mockService.Setup(m => m.AddRewardTransactionForApplyCoupon(
                It.IsAny<Guid>(),
                It.IsAny<CouponDiscountType>(),
                It.IsAny<double>(),
                It.IsAny<int>())).ReturnsAsync(returnFailedServiceResponse);
            //must pass in params like this
            RewardTransactionForApplyCouponAddRequestDto requestDto = new()
            {
                UserId = Guid.NewGuid(),
                DiscountType = Models.CouponDiscountType.ByValue,
                DiscountValue = 200,
                PointTransition = -20
            };
            RewardTransactionController controller = new RewardTransactionController(mockService.Object, mockMapper.Object);

            //Act
            var backChannelResult = await controller.AddRewardTransactionForApplyCoupon(requestDto);

            //Assert
            Assert.NotNull(backChannelResult);
            backChannelResult.Should().BeOfType<BackChannelResponseDto<RewardTransactionDto>>();
            backChannelResult.IsSuccess.Should().BeFalse();
            backChannelResult.IsFailed.Should().BeTrue();
            backChannelResult.Result.Should().BeOneOf(ResultType.Failed);
            backChannelResult.Data.Should().BeNull();
            backChannelResult.Error.Should().BeEquivalentTo("any message");

        }

        [Fact]
        public async Task WhenAddRewardTransForCompleteOrderingFailed_ReturnFailedBackChannelResult()
        {
            //Arrange all dependencies & used method
            Mock<IRewardTransactionService> mockService = new Mock<IRewardTransactionService>();
            Mock<IMapper> mockMapper = new Mock<IMapper>();
            ServiceResponseDto<RewardTransaction> returnFailedServiceResponse = ServiceResponseDto<RewardTransaction>.Failure("any message");
            mockService.Setup(m => m.AddRewardTransactionForCompleteOrdering(
                It.IsAny<Guid>(),
                It.IsAny<int>(),
                It.IsAny<double>())).ReturnsAsync(returnFailedServiceResponse);
            //must pass in params like this
            RewardTransactionForCompleteOrderingAddRequestDto requestDto = new()
            {
                UserId = Guid.NewGuid(),
                OrderPrice = 20000,
                PointTransition = 5
            };
            RewardTransactionController controller = new RewardTransactionController(mockService.Object, mockMapper.Object);

            //Act
            var backChannelResult = await controller.AddRewardTransactionForCompleteOrdering(requestDto);

            //Assert
            Assert.NotNull(backChannelResult);
            backChannelResult.Should().BeOfType<BackChannelResponseDto<RewardTransactionDto>>();
            backChannelResult.IsSuccess.Should().BeFalse();
            backChannelResult.IsFailed.Should().BeTrue();
            backChannelResult.Result.Should().BeOneOf(ResultType.Failed);
            backChannelResult.Data.Should().BeNull();
            backChannelResult.Error.Should().BeEquivalentTo("any message");
        }

        [Fact]
        public async Task WhenAddRewardTransForCompleteOrderingSuccess_ReturnSuccessBackChannelResult()
        {
            //Arrange all dependencies & used method
            Mock<IRewardTransactionService> mockService = new Mock<IRewardTransactionService>();
            Mock<IMapper> mockMapper = new Mock<IMapper>();
            RewardTransactionDto returnRewardTransDto = new RewardTransactionDto()
            {
                UserId = Guid.Parse("93d3b6d6-0187-4a87-be75-57f16e9eb253"),
                DateTransition = new DateTime(2024, 12, 30, 2, 2, 2, DateTimeKind.Utc),
                PointAfterTransaction = 30,
                PointBeforeTransaction = 25,
                PointTransition = 5,
                RewardTransactionId = Guid.Parse("c574afbc-3285-4de6-8230-3970605e176f"),
                Origin = new OriginJson()
                {
                    Reason = Reason.Order,
                    OrderPrice = 20000
                }
            };
            ServiceResponseDto<RewardTransaction> returnSuccessServiceResponse = ServiceResponseDto<RewardTransaction>.Success(new RewardTransaction()
            {
                UserId = Guid.Parse("93d3b6d6-0187-4a87-be75-57f16e9eb253"),
                DateTransition = new DateTime(2024, 12, 30, 2, 2, 2, DateTimeKind.Utc),
                PointAfterTransaction = 30,
                PointBeforeTransaction = 25,
                PointTransition = 5,
                RewardTransactionId = Guid.Parse("c574afbc-3285-4de6-8230-3970605e176f"),
                Origin = new OriginJson()
                {
                    Reason = Reason.Order,
                    OrderPrice = 20000
                }
            });
            mockService.Setup(m => m.AddRewardTransactionForCompleteOrdering(It.IsAny<Guid>(),
                It.IsAny<int>(),
                It.IsAny<double>())).ReturnsAsync(returnSuccessServiceResponse);
            //must pass in params like this
            mockMapper.Setup(m => m.Map<RewardTransactionDto>(It.IsAny<RewardTransaction>())).Returns(returnRewardTransDto);
            RewardTransactionForCompleteOrderingAddRequestDto requestDto = new()
            {
                UserId = Guid.NewGuid(),
                OrderPrice = 20000,
                PointTransition = 5
            };
            RewardTransactionController controller = new RewardTransactionController(mockService.Object, mockMapper.Object);

            //Act
            var backChannelResult = await controller.AddRewardTransactionForCompleteOrdering(requestDto);

            //Assert
            Assert.NotNull(backChannelResult);
            backChannelResult.Should().BeOfType<BackChannelResponseDto<RewardTransactionDto>>();
            backChannelResult.IsSuccess.Should().BeTrue();
            backChannelResult.Result.Should().BeOneOf(ResultType.Success);
            backChannelResult.Data.Should().NotBeNull();
            backChannelResult.Data.Should().BeAssignableTo<RewardTransactionDto>();
            backChannelResult.Data.Should().BeEquivalentTo(returnRewardTransDto);

        }
    }
}
