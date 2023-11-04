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
        #region Sync Methods
        public Coupon Add(Coupon nCoupon)
        {

            _dbContext.Add(nCoupon);
            try
            {
                _dbContext.SaveChanges();
                return nCoupon; //luc nay da duoc set , ke ca guid
            }
            catch (Exception ex) { throw ex; }
            
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
        #endregion

        #region Async Methods
        public async Task<Coupon> AddAsync(Coupon nCoupon)
        {

            await _dbContext.AddAsync(nCoupon);
            try {
                await _dbContext.SaveChangesAsync();
                return nCoupon; //luc nay da duoc set , ke ca guid
            }
            catch (Exception ex) { 
                throw ex; 
            }

        }

        public async Task<Coupon> DeleteAsync(Guid id)
        {
            var couponDel = await this.GetAsync(id);
            if (couponDel == null) {
                throw new Exception("coupon not exist");
                return null;
            }
            try {
                _dbContext.Remove(couponDel);
                _dbContext.SaveChanges();
                return couponDel;
            }
            catch { return null; }

        }

        public async Task<Coupon> GetAsync(Guid couponId)
        {
            Coupon coupon = await _dbContext.Coupons.FindAsync(couponId);
            return coupon == null ? null : coupon;
        }

        public async Task<Coupon> UpdateAsync(Coupon uCoupon)
        {
            try
            {
                _dbContext.Update(uCoupon);
                await _dbContext.SaveChangesAsync();
                return uCoupon;
            }
            catch { 
                return null; 
            }
        }
        #endregion

        public IQueryable<Coupon> GetAsQueryable()
        {
            return _dbContext.Coupons.AsQueryable();
        }
    }
}
