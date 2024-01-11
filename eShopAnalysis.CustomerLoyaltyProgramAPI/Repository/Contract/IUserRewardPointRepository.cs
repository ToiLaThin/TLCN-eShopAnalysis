using eShopAnalysis.CustomerLoyaltyProgramAPI.Models;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.Repository
{
    public interface IUserRewardPointRepository
    {
        IQueryable<UserRewardPoint> GetAsQueryable();
        UserRewardPoint Get(Guid userId);

        //entity should be find and add to chnage tracking before use update & delete
        UserRewardPoint Add(UserRewardPoint nUserRewardPoint);

        UserRewardPoint Update(UserRewardPoint uUserRewardPoint);

        UserRewardPoint Delete(UserRewardPoint delUserRewardPoint);

        Task<UserRewardPoint> GetAsync(Guid userId);

        Task<UserRewardPoint> AddAsync(UserRewardPoint nUserRewardPoint);

    }
}
