using eShopAnalysis.CustomerLoyaltyProgramAPI.Models;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.Repository
{
    public interface IRewardTransactionRepository
    {
        IQueryable<RewardTransaction> GetAsQueryable();
        RewardTransaction Get(Guid rewardTransactionId);

        RewardTransaction Add(RewardTransaction nRewardTransaction);

        //entity should be find and add to chnage tracking before use update & delete
        RewardTransaction Update(RewardTransaction uRewardTransaction);

        RewardTransaction Delete(RewardTransaction delRewardTransaction);

        Task<RewardTransaction> GetAsync(Guid rewardTransactionId);

        Task<RewardTransaction> AddAsync(RewardTransaction nRewardTransaction);


    }
}
