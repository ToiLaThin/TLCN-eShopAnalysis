using eShopAnalysis.CustomerLoyaltyProgramAPI.Data;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Models;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Repository;
using Microsoft.EntityFrameworkCore;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.Repository
{
    public class UserRewardPointRepository : IUserRewardPointRepository
    {
        private readonly PostgresDbContext _dbContext;
        public UserRewardPointRepository(PostgresDbContext dbContext) { 
            _dbContext = dbContext;
        }
        public UserRewardPoint Add(UserRewardPoint nUserRewardPoint)
        {
            var addedUserRewardPoint = _dbContext.UserRewardPoints.Add(nUserRewardPoint).Entity;
            return addedUserRewardPoint;
        }

        public async Task<UserRewardPoint> AddAsync(UserRewardPoint nUserRewardPoint)
        {
            var addedUserRewardPointEntity = await _dbContext.UserRewardPoints.AddAsync(nUserRewardPoint);
            return addedUserRewardPointEntity.Entity;
        }

        public UserRewardPoint Delete(UserRewardPoint delUserRewardPoint)
        {
            var deletedUserRewardPoint = _dbContext.UserRewardPoints.Remove(delUserRewardPoint).Entity;
            return deletedUserRewardPoint;
        }

        public UserRewardPoint Get(Guid userId)
        {
            UserRewardPoint userRewardPoint = _dbContext.UserRewardPoints.Find(userId);
            return userRewardPoint;
        }

        public IQueryable<UserRewardPoint> GetAsQueryable()
        {
            var queryableUserRewardPoints = _dbContext.UserRewardPoints.AsQueryable();
            return queryableUserRewardPoints;
        }

        public async Task<UserRewardPoint> GetAsync(Guid userId)
        {
            UserRewardPoint userRewardPoint = await _dbContext.UserRewardPoints.FindAsync(userId);
            return userRewardPoint;
        }

        //entity should be find and add to chnage tracking before use update & delete
        public UserRewardPoint Update(UserRewardPoint uUserRewardPoint)
        {
            _dbContext.Entry(uUserRewardPoint).State = EntityState.Modified;
            return uUserRewardPoint;
        }

    }
}
