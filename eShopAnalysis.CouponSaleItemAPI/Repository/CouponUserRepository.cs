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

        public IEnumerable<CouponUser> GetAll()
        {
            var result = _dbContext.CouponUsers.Include(cU => cU.CouponUsed).ToList();
            return result;
        }

    }
}
