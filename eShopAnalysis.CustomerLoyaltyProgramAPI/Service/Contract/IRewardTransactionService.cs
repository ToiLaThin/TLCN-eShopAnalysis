using eShopAnalysis.CustomerLoyaltyProgramAPI.Dto;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Models;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.Service
{
    //add a few more overload method to get the rewardBalanceBefore from frontend
    public interface IRewardTransactionService
    {
        Task<ServiceResponseDto<RewardTransaction>> GetRewardTransaction(Guid rewardTransactionId);

        Task<ServiceResponseDto<IEnumerable<RewardTransaction>>> GetRewardTransactionsOfUser(Guid userId);

        Task<ServiceResponseDto<RewardTransaction>> AddRewardTransactionForApplyCoupon(
            Guid userId,
            CouponDiscountType couponDiscountType,
            double discountValue,
            int pointTransition);

        Task<ServiceResponseDto<RewardTransaction>> AddRewardTransactionForCompleteOrdering(
            Guid userId,
            int pointTransition,
            double orderPrice);

        //please change this
        //Task<ServiceResponseDto<RewardTransaction>> UpdateExistingRewardTransactionInstance(Guid userId, int newRewardPoint);

        Task<ServiceResponseDto<RewardTransaction>> DeleteExistingRewardTransactionInstance(Guid rewardTransactionId);
    }
}
