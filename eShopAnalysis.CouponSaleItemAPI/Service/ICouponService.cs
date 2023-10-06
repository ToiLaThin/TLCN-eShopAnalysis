using eShopAnalysis.CouponSaleItemAPI.Dto;
using eShopAnalysis.CouponSaleItemAPI.Models;

namespace eShopAnalysis.CouponSaleItemAPI.Service
{
    public interface ICouponService
    {
        Task<ServiceResponseDto<Coupon>> Add(Coupon coupon);
        Task<ServiceResponseDto<Coupon>> Delete(Guid coupon);
        ServiceResponseDto<IEnumerable<Coupon>> GetAll();
        ServiceResponseDto<Coupon> GetCoupon(Guid couponId);
    }
}