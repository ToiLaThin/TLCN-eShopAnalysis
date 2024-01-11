using eShopAnalysis.CouponSaleItemAPI.UnitOfWork;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Dto;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Models;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.Service
{
    public class UserRewardPointService : IUserRewardPointService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserRewardPointService(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponseDto<UserRewardPoint>> AddInstance(Guid userId, int initRewardPoint = 0)
        {
            bool isUserHaveRewardPointInstance = await _unitOfWork.UserRewardPointRepository.GetAsync(userId) != null;
            if (isUserHaveRewardPointInstance) {
                return ServiceResponseDto<UserRewardPoint>.Failure("Cannot add this user reward instance because user already have one instance existed");
            }

            UserRewardPoint newUserRewardPoint = new UserRewardPoint() { UserId = userId, RewardPoint = initRewardPoint };
            UserRewardPoint addedUserRewardPoint = await _unitOfWork.UserRewardPointRepository.AddAsync(newUserRewardPoint);
            if (addedUserRewardPoint == null) {
                return ServiceResponseDto<UserRewardPoint>.Failure("Cannot add this user reward instance");
            }
            return ServiceResponseDto<UserRewardPoint>.Success(addedUserRewardPoint);
        }

        public async Task<ServiceResponseDto<UserRewardPoint>> DeleteExistingInstance(Guid userId)
        {
            UserRewardPoint userRewardPointToDel = await _unitOfWork.UserRewardPointRepository.GetAsync(userId);
            if (userRewardPointToDel == null) {
                return ServiceResponseDto<UserRewardPoint>.Failure("Cannot delete this user reward instance because it is not existed");
            }

            UserRewardPoint deletedUserRewardPoint = _unitOfWork.UserRewardPointRepository.Delete(userRewardPointToDel);
            return ServiceResponseDto<UserRewardPoint>.Success(deletedUserRewardPoint);
        }

        public async Task<ServiceResponseDto<UserRewardPoint>> GetRewardPointOfUser(Guid userId)
        {
            UserRewardPoint userRewardPoint = await _unitOfWork.UserRewardPointRepository.GetAsync(userId);
            if (userRewardPoint == null) {
                return ServiceResponseDto<UserRewardPoint>.Failure("Cannot find reward point instance of this user");
            }
            return ServiceResponseDto<UserRewardPoint>.Success(userRewardPoint);
        }

        public async Task<ServiceResponseDto<UserRewardPoint>> UpdateExistingInstance(Guid userId, int newRewardPoint)
        {
            UserRewardPoint userRewardPointToUpdate = await _unitOfWork.UserRewardPointRepository.GetAsync(userId);
            if (userRewardPointToUpdate == null) {
                return ServiceResponseDto<UserRewardPoint>.Failure("Cannot update this user reward instance because it is not existed");
            }

            UserRewardPoint updatedUserRewardPoint = _unitOfWork.UserRewardPointRepository.Update(userRewardPointToUpdate);
            return ServiceResponseDto<UserRewardPoint>.Success(updatedUserRewardPoint);
        }
    }
}
