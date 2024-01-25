using eShopAnalysis.CustomerLoyaltyProgramAPI.Models;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Utilities.Factory;
using FluentAssertions;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.UnitTest.Utils
{
    //test class must be public
    public class UnitTestRewardTransactionFactory
    {
        protected RewardTransactionFactory RewardTransactionFactory { get; set; }
        public UnitTestRewardTransactionFactory() {
            RewardTransactionFactory = new RewardTransactionFactory();
        }

        [Theory]
        [MemberData(nameof(ValidTestCaseCreateRewardTransForApplyCoupon))]
        public void GivenTheValidTestCase_WhenCreateRewardTransForApplyCoupon_ReturnValidRewardTrans(Guid userId,
            int pointTransition,
            int balanceBefore,
            CouponDiscountType couponDiscountType,
            double discountValue)
        {
            //Arrange
            DateTime fixedDate = DateTime.Now;
            Guid fixedRewardTransId = Guid.NewGuid();
            RewardTransaction expectedRewardTransaction = new RewardTransaction()
            {
                UserId = userId,
                PointTransition = pointTransition,
                PointAfterTransaction = balanceBefore + pointTransition,
                PointBeforeTransaction = balanceBefore,
                DateTransition = fixedDate,
                RewardTransactionId = fixedRewardTransId
            };

            //Act
            RewardTransaction actualRewardTransaction = RewardTransactionFactory.ProduceRewardTransactionDecrByApplyCoupon(userId, pointTransition, balanceBefore, couponDiscountType, discountValue);
            actualRewardTransaction.RewardTransactionId = fixedRewardTransId;
            actualRewardTransaction.DateTransition = fixedDate;

            //Assert 
            Assert.NotNull(actualRewardTransaction);
            actualRewardTransaction.Should().BeOfType<RewardTransaction>();
            actualRewardTransaction.Should().BeAssignableTo<RewardTransaction>();

            actualRewardTransaction.Should().BeEquivalentTo(actualRewardTransaction);
            actualRewardTransaction.Origin.Should().Match<OriginJson>(o => o.Reason == Reason.ApplyCoupon && o.DiscountValue == discountValue && o.DiscountType == couponDiscountType);
            actualRewardTransaction.Origin.OrderPrice.Should().BeNull();

        
        
        }


        [Theory]
        [MemberData(nameof(InValidTestCaseCreateRewardTransForApplyCoupon))]
        public void GivenTheInValidTestCase_WhenCreateRewardTransForApplyCoupon_ReturnTestWithThrownInvalidArgException(Guid userId,
            int pointTransition,
            int balanceBefore,
            CouponDiscountType couponDiscountType,
            double discountValue)
        {
            //Arrange


            //Act

            //Assert 
            Assert.Throws<ArgumentException>(() => RewardTransactionFactory.ProduceRewardTransactionDecrByApplyCoupon(userId, pointTransition, balanceBefore, couponDiscountType, discountValue));



        }

        public static IEnumerable<object[]> ValidTestCaseCreateRewardTransForApplyCoupon => new[]
        {
            new object[] { Guid.NewGuid(), -5, 20, CouponDiscountType.ByPercent, 20 },
            new object[] { Guid.NewGuid(), -5, 5, CouponDiscountType.ByPercent, 30 },
            new object[] { Guid.NewGuid(), -5, 20, CouponDiscountType.ByValue, 4000 },
        };

        public static IEnumerable<object[]> InValidTestCaseCreateRewardTransForApplyCoupon => new[]
        {
            new object[] { Guid.NewGuid(), 5, 20, CouponDiscountType.ByPercent, 20 },
            new object[] { Guid.NewGuid(), -5, 2, CouponDiscountType.ByPercent, 30 },
            new object[] { Guid.NewGuid(), -5, 20, -1, 4000 },
            //value is not in range with that coupon type, value is < 0
        };



        //////////

        [Theory]
        [MemberData(nameof(ValidTestCaseCreateRewardTransForOrder))]
        public void GivenTheValidTestCase_WhenCreateRewardTransForOrder_ReturnValidRewardTrans(Guid userId,
            int pointTransition,
            int balanceBefore,
            double orderPrice)
        {
            //Arrange
            DateTime fixedDate = DateTime.Now;
            Guid fixedRewardTransId = Guid.NewGuid();
            RewardTransaction expectedRewardTransaction = new RewardTransaction()
            {
                UserId = userId,
                PointTransition = pointTransition,
                PointAfterTransaction = balanceBefore + pointTransition,
                PointBeforeTransaction = balanceBefore,
                DateTransition = fixedDate,
                RewardTransactionId = fixedRewardTransId
            };

            //Act
            RewardTransaction actualRewardTransaction = RewardTransactionFactory.ProduceRewardTransactionIncrByOrder(userId, pointTransition, balanceBefore, orderPrice);
            actualRewardTransaction.RewardTransactionId = fixedRewardTransId;
            actualRewardTransaction.DateTransition = fixedDate;

            //Assert 
            Assert.NotNull(actualRewardTransaction);
            actualRewardTransaction.Should().BeOfType<RewardTransaction>();
            actualRewardTransaction.Should().BeAssignableTo<RewardTransaction>();

            actualRewardTransaction.Should().BeEquivalentTo(actualRewardTransaction);
            actualRewardTransaction.Origin.Should().Match<OriginJson>(o => o.Reason == Reason.Order && o.OrderPrice == orderPrice);
            actualRewardTransaction.Origin.DiscountValue.Should().BeNull();
            actualRewardTransaction.Origin.DiscountType.Should().BeNull();



        }


        [Theory]
        [MemberData(nameof(InValidTestCaseCreateRewardTransForOrder))]
        public void GivenTheInValidTestCase_WhenCreateRewardTransForOrder_ReturnTestWithThrownInvalidArgException(Guid userId,
            int pointTransition,
            int balanceBefore,
            double orderPrice)
        {
            //Arrange


            //Act

            //Assert 
            Assert.Throws<ArgumentException>(() => RewardTransactionFactory.ProduceRewardTransactionIncrByOrder(userId, pointTransition, balanceBefore, orderPrice));



        }

        public static IEnumerable<object[]> ValidTestCaseCreateRewardTransForOrder => new[]
        {
            new object[] { Guid.NewGuid(), 5, 20, 20000 },
            new object[] { Guid.NewGuid(), 2, 5, 5000 },
            new object[] { Guid.NewGuid(), 100, 20, 20000 },
        };

        public static IEnumerable<object[]> InValidTestCaseCreateRewardTransForOrder => new[]
        {
            //point transition is <= 0
            new object[] { Guid.NewGuid(), -5, 20, 2000 },
            new object[] { Guid.NewGuid(), 0, 20, 2000 },
            //balance before is < 0
            new object[] { Guid.NewGuid(), 5, -2, 2000 },
            //order price is <= 0
            new object[] { Guid.NewGuid(), 0, 20, -2000 },
            new object[] { Guid.NewGuid(), 0, 20, 0 },
            //user id is of wrong type
        };

    }
}
