using eShopAnalysis.CouponSaleItemAPI.Models;

namespace eShopAnalysis.CouponSaleItemAPI.Service
{
    public interface ICouponService
    {
        Task<Coupon> Add(Coupon coupon);
        Task<Coupon> Delete(Guid coupon);
        IEnumerable<Coupon> GetAll();
        Coupon GetCoupon(Guid couponId);
    }
}