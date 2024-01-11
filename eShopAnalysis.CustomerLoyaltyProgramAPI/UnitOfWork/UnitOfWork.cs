using eShopAnalysis.CustomerLoyaltyProgramAPI.Data;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Repository;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace eShopAnalysis.CouponSaleItemAPI.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        //since this is microservice,the number of repo is limited
        //so i do not use generic repository
        //unit of work is really not necessary in this CouponSaleItem Api
        //it is needed typically in order or shopping car where tables
        //have relationship and multiple entity might be add or delete at the same time

        //but unit of work in CQRS or DDD MIGHT not be implemented this way since it might have many side effect(domain events,....)

        private PostgresDbContext _dbContext;
        private IUserRewardPointRepository _userRewardPointRepository;
        private IRewardTransactionRepository _rewardTransactionRepository;
        private IDbContextTransaction _currentTransaction;
        public UnitOfWork(PostgresDbContext dbContext) { _dbContext = dbContext; }

        public IUserRewardPointRepository UserRewardPointRepository {
            //share context between every repo
            get {
                if (_userRewardPointRepository == null) {
                    this._userRewardPointRepository = new UserRewardPointRepository(_dbContext);
                }
                return _userRewardPointRepository;
            }
        }

        public IRewardTransactionRepository RewardTransactionRepository {
            get {
                if (_rewardTransactionRepository == null) {
                    this._rewardTransactionRepository = new RewardTransactionRepository(_dbContext);
                }
                return _rewardTransactionRepository;
            }
        }

        public void Dispose()
        {
            _dbContext.Dispose();
            GC.SuppressFinalize(this);
        }

        public bool HasActiveTransaction() => _currentTransaction != null;

        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null) return null;
            _currentTransaction = await _dbContext.Database.BeginTransactionAsync();
            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) return;
            try
            {
                await this._dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
    }
}
