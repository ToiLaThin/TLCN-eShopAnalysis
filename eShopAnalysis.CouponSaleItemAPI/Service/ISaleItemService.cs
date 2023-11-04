using eShopAnalysis.CouponSaleItemAPI.Dto;
using eShopAnalysis.CouponSaleItemAPI.Models;

namespace eShopAnalysis.CouponSaleItemAPI.Service
{
    public interface ISaleItemService
    {
        Task<ServiceResponseDto<SaleItem>> Add(SaleItem saleItem);
        Task<ServiceResponseDto<SaleItem>> Delete(Guid saleItem);
        Task<ServiceResponseDto<IEnumerable<SaleItem>>> GetAll();
        Task<ServiceResponseDto<SaleItem>> GetCoupon(Guid couponId);
    }
}