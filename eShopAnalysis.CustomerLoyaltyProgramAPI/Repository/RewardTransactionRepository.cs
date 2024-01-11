using eShopAnalysis.CustomerLoyaltyProgramAPI.Data;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Models;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Repository;
using Microsoft.EntityFrameworkCore;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.Repository
{
    public class RewardTransactionRepository : IRewardTransactionRepository
    {
        private readonly PostgresDbContext _dbContext;
        public RewardTransactionRepository(PostgresDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public RewardTransaction Add(RewardTransaction nRewardTransaction)
        {
            var addedRewardTransation = _dbContext.RewardTransactions.Add(nRewardTransaction).Entity;
            return addedRewardTransation;
        }

        public async Task<RewardTransaction> AddAsync(RewardTransaction nRewardTransaction)
        {
            var addedRewardTransationEntity = await _dbContext.RewardTransactions.AddAsync(nRewardTransaction);
            return addedRewardTransationEntity.Entity;
        }

        public RewardTransaction Delete(RewardTransaction delRewardTransaction)
        {
            var deletedRewardTransaction = _dbContext.RewardTransactions.Remove(delRewardTransaction).Entity;
            return deletedRewardTransaction;
        }

        public RewardTransaction Get(Guid rewardTransactionId)
        {
            RewardTransaction rewardTransaction = _dbContext.RewardTransactions.Find(rewardTransactionId);
            return rewardTransaction;
        }

        public IQueryable<RewardTransaction> GetAsQueryable()
        {
            var queryableRewardTransactions = _dbContext.RewardTransactions.AsQueryable();
            return queryableRewardTransactions;
        }

        public async Task<RewardTransaction> GetAsync(Guid rewardTransactionId)
        {
            RewardTransaction rewardTransaction = await _dbContext.RewardTransactions.FindAsync(rewardTransactionId);
            return rewardTransaction;
        }

        //entity should be find and add to chnage tracking before use update & delete
        public RewardTransaction Update(RewardTransaction uRewardTransaction)
        {
            _dbContext.Entry(uRewardTransaction).State = EntityState.Modified;
            return uRewardTransaction;
        }

        
    }
}
