using eShopAnalysis.CouponSaleItemAPI.Data;
using eShopAnalysis.CouponSaleItemAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace eShopAnalysis.CouponSaleItemAPI.Repository
{
    public class CouponUserRepository : ICouponUserRepository
    {
        private readonly PostgresDbContext _dbContext;
        public CouponUserRepository(PostgresDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region Sync Methods
        public CouponUser Add(CouponUser couponUser)
        {
            _dbContext.Add(couponUser);
            try
            {
                _dbContext.SaveChanges();
                return couponUser; //luc nay da duoc set , ke ca guid
            }
            catch (Exception ex) { 
                string msg = ex.Message;
                return null; 
            }
        }
        public CouponUser Delete(Guid couponId, Guid userId)
        {
            var couponUserDel = this.Get(couponId, userId);
            if (couponUserDel == null)
            {
                throw new Exception("coupon user not exist");
                return null;
            }
            try
            {
                _dbContext.Remove(couponUserDel);
                _dbContext.SaveChanges();
                return couponUserDel;
            }
            catch { return null; }

        }

        public CouponUser Get(Guid couponId, Guid userId)
        {
            var couponUser = _dbContext.CouponUsers.Include(cU => cU.CouponUsed).FirstOrDefault(cU => cU.UserId == userId && cU.CouponId == couponId);
            return couponUser == null ? null : couponUser;
        }
        #endregion

        #region Async Methods
        public async Task<CouponUser> AddAsync(CouponUser couponUser)
        {
            await _dbContext.AddAsync(couponUser);
            try {
                await _dbContext.SaveChangesAsync();
                return couponUser; //luc nay da duoc set , ke ca guid
            }
            catch (Exception ex) {
                string msg = ex.Message;
                return null;
            }
        }
        public async Task<CouponUser> DeleteAsync(Guid couponId, Guid userId)
        {
            //should be tracked by change tracker, so notice the get method
            var couponUserDel = await this.GetAsync(couponId, userId);
            if (couponUserDel == null) {
                throw new Exception("coupon user not exist");
                return null;
            }
            try {
                _dbContext.Remove(couponUserDel);
                await _dbContext.SaveChangesAsync();
                return couponUserDel;
            }
            catch { return null; }

        }

        public async Task<CouponUser> GetAsync(Guid couponId, Guid userId)
        {
            var couponUser = await _dbContext.CouponUsers.Include(cU => cU.CouponUsed)
                                                         .FirstOrDefaultAsync(cU => cU.UserId == userId && cU.CouponId == couponId);
            return couponUser == null ? null : couponUser;
        }
        #endregion

        public IQueryable<CouponUser> GetAsQueryableIncludedCouponUsed()
        {
            //this is eager load, when call to list in service layer, will query couponUsed too
            var result = _dbContext.CouponUsers.Include(cU => cU.CouponUsed).AsQueryable();
            return result;
        }

        public IQueryable<CouponUser> GetAsQueryable()
        {
            //this is eager load, when call to list in service layer, will query couponUsed too
            //should add as no tracking in the service layer if needed
            var result = _dbContext.CouponUsers.AsQueryable();
            return result;
        }

    }
}
