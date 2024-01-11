using eShopAnalysis.CustomerLoyaltyProgramAPI.Models;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.Utilities.Factory
{
    public class RewardTransactionFactory : IRewardTransactionFactory
    {
        public RewardTransactionFactory() {
        }

        public RewardTransaction ProduceRewardTransactionIncrByOrder(Guid userId, int pointTransition, int balanceBefore, double orderPrice) 
        {
            if (pointTransition <= 0) {
                throw new ArgumentException("the point transition is not valid");
            }

            if (balanceBefore < 0) {
                throw new ArgumentException("invalid balance , it is less than zero ");
            }

            return new RewardTransaction() {
                RewardTransactionId = Guid.NewGuid(),
                UserId = userId,
                PointTransition = pointTransition,
                DateTransition = DateTime.UtcNow,
                PointBeforeTransaction = balanceBefore,
                PointAfterTransaction = balanceBefore + pointTransition,
                Origin = new OriginJson()
                {
                    Reason = Reason.Order,
                    OrderPrice = orderPrice,
                    DiscountType = null,
                    DiscountValue = null
                }
            };
        }

        public RewardTransaction ProduceRewardTransactionDecrByApplyCoupon(Guid userId,
            int pointTransition,
            int balanceBefore,
            CouponDiscountType couponDiscountType,
            double discountValue)
        {
            if (pointTransition >= 0) {
                throw new ArgumentException("the point transition is not valid");
            }

            if (balanceBefore + pointTransition < 0) {
                throw new ArgumentException("the transaction is not valid, it cause balance to be less than zero ");
            }

            return new RewardTransaction()
            {
                RewardTransactionId = Guid.NewGuid(),
                UserId = userId,
                PointTransition = pointTransition,
                DateTransition = DateTime.UtcNow,
                PointBeforeTransaction = balanceBefore,
                PointAfterTransaction = balanceBefore + pointTransition,
                Origin = new OriginJson() {
                    Reason = Reason.ApplyCoupon,
                    OrderPrice = null,
                    DiscountType = couponDiscountType,
                    DiscountValue = discountValue
                }
            };
        }
    }
}

