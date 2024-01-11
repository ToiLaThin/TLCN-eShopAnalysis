using eShopAnalysis.CouponSaleItemAPI.UnitOfWork;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Dto;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Models;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Utilities.Factory;
using Microsoft.EntityFrameworkCore;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.Service
{
    public class RewardTransactionService : IRewardTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRewardTransactionFactory _rewardTransactionFactory;

        public RewardTransactionService(IUnitOfWork unitOfWork, IRewardTransactionFactory rewardTransactionFactory)
        {
            _unitOfWork = unitOfWork;
            _rewardTransactionFactory = rewardTransactionFactory;
        }

        //use both repo, use user reward repo to get the current point first, or we can pass it from the frontend
        public async Task<ServiceResponseDto<RewardTransaction>> AddRewardTransactionForApplyCoupon(Guid userId, CouponDiscountType couponDiscountType, double discountValue, int pointTransition)
        {
            var transaction = await _unitOfWork.BeginTransactionAsync();
            UserRewardPoint userRewardPoint = await _unitOfWork.UserRewardPointRepository.GetAsync(userId);
            if (userRewardPoint == null) {
                return ServiceResponseDto<RewardTransaction>.Failure("cannot add reward transaction, the user reward point is not exist, please create one");
            }
            int rewardBalanceBefore = userRewardPoint.RewardPoint;

            RewardTransaction rewardTransactionToAdd = _rewardTransactionFactory.ProduceRewardTransactionDecrByApplyCoupon(
                userId: userId,
                pointTransition: pointTransition,
                balanceBefore: rewardBalanceBefore,
                couponDiscountType: couponDiscountType,
                discountValue: discountValue);
            RewardTransaction rewardTransactionAdded = await _unitOfWork.RewardTransactionRepository.AddAsync(rewardTransactionToAdd);


            if (rewardTransactionAdded == null) {
                _unitOfWork.RollbackTransaction();
                return ServiceResponseDto<RewardTransaction>.Failure("cannot add reward transaction, please check repository result");
            }

            //update point in user reward
            userRewardPoint.RewardPoint = rewardTransactionAdded.PointAfterTransaction;
            //already tracked by change tracking
            _unitOfWork.UserRewardPointRepository.Update(userRewardPoint);
            await _unitOfWork.CommitTransactionAsync(transaction);
            return ServiceResponseDto<RewardTransaction>.Success(rewardTransactionAdded);
        }

        public async Task<ServiceResponseDto<RewardTransaction>> AddRewardTransactionForCompleteOrdering(Guid userId, int pointTransition, double orderPrice)
        {
            var transaction = await _unitOfWork.BeginTransactionAsync();
            UserRewardPoint userRewardPoint = await _unitOfWork.UserRewardPointRepository.GetAsync(userId);
            int rewardBalanceBefore = userRewardPoint.RewardPoint;

            RewardTransaction rewardTransactionToAdd = _rewardTransactionFactory.ProduceRewardTransactionIncrByOrder(
                userId: userId,
                pointTransition: pointTransition,
                balanceBefore: rewardBalanceBefore,
                orderPrice: orderPrice);
            RewardTransaction rewardTransactionAdded = await _unitOfWork.RewardTransactionRepository.AddAsync(rewardTransactionToAdd);


            if (rewardTransactionAdded == null)
            {
                _unitOfWork.RollbackTransaction();
                return ServiceResponseDto<RewardTransaction>.Failure("cannot add reward transaction, please check repository result");
            }
            //update point in user reward
            userRewardPoint.RewardPoint = rewardTransactionAdded.PointAfterTransaction;
            //already tracked by change tracking
            _unitOfWork.UserRewardPointRepository.Update(userRewardPoint);
            await _unitOfWork.CommitTransactionAsync(transaction);
            return ServiceResponseDto<RewardTransaction>.Success(rewardTransactionAdded);
        }

        public async Task<ServiceResponseDto<RewardTransaction>> DeleteExistingRewardTransactionInstance(Guid rewardTransactionId)
        {
            var rewardTransaction = await _unitOfWork.RewardTransactionRepository.GetAsync(rewardTransactionId);
            if (rewardTransaction == null) {
                return ServiceResponseDto<RewardTransaction>.Failure("cannot find the reward transaction to delete");
            }

            var transaction = await _unitOfWork.BeginTransactionAsync();
            var deletedTransaction = _unitOfWork.RewardTransactionRepository.Delete(rewardTransaction);
            if (deletedTransaction == null) {
                _unitOfWork.RollbackTransaction();
                return ServiceResponseDto<RewardTransaction>.Failure("cannot delete reward transaction");
            }

            await _unitOfWork.CommitTransactionAsync(transaction);
            return ServiceResponseDto<RewardTransaction>.Success(deletedTransaction);
        }

        public async Task<ServiceResponseDto<RewardTransaction>> GetRewardTransaction(Guid rewardTransactionId)
        {
            var rewardTransaction = await _unitOfWork.RewardTransactionRepository.GetAsync(rewardTransactionId);
            if (rewardTransaction == null) {
                return ServiceResponseDto<RewardTransaction>.Failure("cannot find the reward transaction");
            }
            return ServiceResponseDto<RewardTransaction>.Success(rewardTransaction);
        }

        public async Task<ServiceResponseDto<IEnumerable<RewardTransaction>>> GetRewardTransactionsOfUser(Guid userId)
        {
            var rewardTransactionsOfUser = await _unitOfWork.RewardTransactionRepository.GetAsQueryable()
                                                                                  .Where(rt => rt.UserId.Equals(userId))
                                                                                  .ToListAsync();
            if (rewardTransactionsOfUser == null) {
                return ServiceResponseDto<IEnumerable<RewardTransaction>>.Failure("the list is null, which is invalid");
            }
            return ServiceResponseDto<IEnumerable<RewardTransaction>>.Success(rewardTransactionsOfUser);
        }
    }
}
