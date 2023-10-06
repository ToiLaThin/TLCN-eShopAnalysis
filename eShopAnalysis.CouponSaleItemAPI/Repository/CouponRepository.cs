using eShopAnalysis.CouponSaleItemAPI.Data;
using eShopAnalysis.CouponSaleItemAPI.Models;

namespace eShopAnalysis.CouponSaleItemAPI.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly PostgresDbContext _dbContext;
        public CouponRepository(PostgresDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //all operation if failed(return null), the service will use unit of work transaction to rollback
        public Coupon Add(Coupon nCoupon)
        {

            _dbContext.Add(nCoupon);
            try
            {
                _dbContext.SaveChanges();
                return nCoupon; //luc nay da duoc set , ke ca guid
            }
            catch { return null; }
            
        }

        public Coupon Delete(Guid id)
        {
            var couponDel = this.Get(id);
            if (couponDel == null) {
                throw new Exception("coupon not exist");
                return null;
            }
            try
            {
                _dbContext.Remove(couponDel);
                _dbContext.SaveChanges();
                return couponDel;
            }
            catch { return null; }

        }

        public Coupon Get(Guid couponId)
        {
            Coupon coupon = _dbContext.Coupons.Find(couponId);
            return coupon == null ? null : coupon;
        }

        public IEnumerable<Coupon> GetAll()
        {
            return _dbContext.Coupons.ToList();
        }

        public IQueryable<Coupon> GetAllAsQueryable()
        {
            return _dbContext.Coupons.AsQueryable();
        }

        public Coupon Update(Coupon uCoupon)
        {
            try
            {
                _dbContext.Update(uCoupon);
                _dbContext.SaveChanges();
                return uCoupon;
            }
            catch { return null; }            
        }
    }
}
