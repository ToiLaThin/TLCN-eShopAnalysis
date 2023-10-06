using eShopAnalysis.CouponSaleItemAPI.Models;
using eShopAnalysis.CouponSaleItemAPI.UnitOfWork;

namespace eShopAnalysis.CouponSaleItemAPI.Service
{
    public class CouponService : ICouponService
    {
        private IUnitOfWork _uOW;
        public CouponService(IUnitOfWork uOW)
        {
            _uOW = uOW;
        }

        public async Task<Coupon> Add(Coupon coupon)
        {
            var transaction = await _uOW.BeginTransactionAsync();
            var result = _uOW.CouponRepository.Add(coupon);
            if (result == null)
            {
                transaction.RollbackAsync();
                return null;
            }
            else
            {
                transaction.CommitAsync();
                return result;
            }
        }

        public async Task<Coupon> Delete(Guid coupon)
        {
            var transaction = await _uOW.BeginTransactionAsync();
            var result = _uOW.CouponRepository.Delete(coupon);
            if (result == null)
            {
                transaction.RollbackAsync();
                return null;
            }
            else
            {
                transaction.CommitAsync();
                return result;
            }
        }

        public IEnumerable<Coupon> GetAll()
        {
            var result = _uOW.CouponRepository.GetAll();
            return result;
        }

        public Coupon GetCoupon(Guid couponId)
        {
            var result = _uOW.CouponRepository.Get(couponId);
            return result;
        }


    }
}
