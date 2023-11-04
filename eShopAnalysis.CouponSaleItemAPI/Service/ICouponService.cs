using eShopAnalysis.CouponSaleItemAPI.Dto;
using eShopAnalysis.CouponSaleItemAPI.Models;

namespace eShopAnalysis.CouponSaleItemAPI.Service
{
    public interface ICouponService
    {
        Task<ServiceResponseDto<Coupon>> Add(Coupon coupon);
        Task<ServiceResponseDto<Coupon>> Delete(Guid coupon);
        Task<ServiceResponseDto<IEnumerable<Coupon>>> GetAll();
        Task<ServiceResponseDto<Coupon>> GetCoupon(Guid couponId);
        Task<ServiceResponseDto<IEnumerable<Coupon>>> GetCouponUsedByUser(Guid userId);
        Task<ServiceResponseDto<IEnumerable<Coupon>>> GetActiveCouponsNotUsedByUser(Guid userId);
        Task<ServiceResponseDto<Coupon>> MarkUserUsedCoupon(Guid userId, Guid couponId);
        Task<ServiceResponseDto<Coupon>> RetrieveValidCouponWithCode(string couponCode);
    }
}