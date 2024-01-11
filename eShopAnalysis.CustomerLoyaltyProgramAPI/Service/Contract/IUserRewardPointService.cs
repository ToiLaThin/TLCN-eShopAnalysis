using eShopAnalysis.CustomerLoyaltyProgramAPI.Dto;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Models;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.Service
{
    public interface IUserRewardPointService
    {
        Task<ServiceResponseDto<UserRewardPoint>> GetRewardPointOfUser(Guid userId);

        Task<ServiceResponseDto<UserRewardPoint>> AddInstance(Guid userId, int initRewardPoint = 0);

        Task<ServiceResponseDto<UserRewardPoint>> UpdateExistingInstance(Guid userId, int newRewardPoint);

        Task<ServiceResponseDto<UserRewardPoint>> DeleteExistingInstance(Guid userId);
    }
}
