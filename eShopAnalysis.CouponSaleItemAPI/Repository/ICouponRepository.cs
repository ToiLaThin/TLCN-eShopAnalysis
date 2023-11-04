using eShopAnalysis.CouponSaleItemAPI.Models;

namespace eShopAnalysis.CouponSaleItemAPI.Repository
{
    public interface ICouponRepository
    {
        IQueryable<Coupon> GetAsQueryable();
        Coupon Get(Guid couponId);

        Coupon Add(Coupon nCoupon);

        Coupon Update(Coupon uCoupon);

        Coupon Delete(Guid id);

        Task<Coupon> GetAsync(Guid couponId);

        Task<Coupon> AddAsync(Coupon nCoupon);

        Task<Coupon> UpdateAsync(Coupon uCoupon);

        Task<Coupon> DeleteAsync(Guid id);
    }
}
