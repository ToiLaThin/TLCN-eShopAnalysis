using eShopAnalysis.CouponSaleItemAPI.Models;

namespace eShopAnalysis.CouponSaleItemAPI.Service
{
    public interface ISaleItemService
    {
        Task<SaleItem> Add(SaleItem saleItem);
        Task<SaleItem> Delete(Guid saleItem);
        IEnumerable<SaleItem> GetAll();
        SaleItem GetCoupon(Guid couponId);
    }
}