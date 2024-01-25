using eShopAnalysis.CustomerLoyaltyProgramAPI.Repository;
using Microsoft.EntityFrameworkCore.Storage;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.UnitOfWork
{
    public interface IUnitOfWork: IDisposable
    {
        public IUserRewardPointRepository UserRewardPointRepository { get; }
        public IRewardTransactionRepository RewardTransactionRepository { get; }
        IDbContextTransaction GetCurrentTransaction();

        bool HasActiveTransaction();

        Task<IDbContextTransaction> BeginTransactionAsync();
        public Task CommitTransactionAsync(IDbContextTransaction transaction);

        public void RollbackTransaction();

        void Dispose();
    }
}
