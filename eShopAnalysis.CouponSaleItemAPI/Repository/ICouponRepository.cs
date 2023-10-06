using eShopAnalysis.CouponSaleItemAPI.Models;

namespace eShopAnalysis.CouponSaleItemAPI.Repository
{
    public interface ICouponRepository
    {
        Coupon Get(Guid couponId);

        IEnumerable<Coupon> GetAll();

        IQueryable<Coupon> GetAllAsQueryable();

        Coupon Add(Coupon nCoupon);

        Coupon Update(Coupon uCoupon);

        Coupon Delete(Guid id);
    }
}
