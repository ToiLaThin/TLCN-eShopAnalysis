using eShopAnalysis.CustomerLoyaltyProgramAPI.Models;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.Utilities.Factory
{
    public interface IRewardTransactionFactory
    {
        RewardTransaction ProduceRewardTransactionIncrByOrder(Guid userId, int pointTransition, int balanceBefore, double orderPrice);

        RewardTransaction ProduceRewardTransactionDecrByApplyCoupon(Guid userId, int pointTransition, int balanceBefore, CouponDiscountType couponDiscountType, double discountValue);
    }
}
