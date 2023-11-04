using eShopAnalysis.CouponSaleItemAPI.Models;

namespace eShopAnalysis.CouponSaleItemAPI.Repository
{
    public interface ISaleItemRepository
    {
        IQueryable<SaleItem> GetAsQueryable();

        SaleItem Get(Guid Id);

        SaleItem Add(SaleItem nSaleItem);

        SaleItem Update(SaleItem uSaleItem);

        SaleItem Delete(Guid Id);

        Task<SaleItem> GetAsync(Guid Id);

        Task<SaleItem> AddAsync(SaleItem nSaleItem);

        Task<SaleItem> UpdateAsync(SaleItem uSaleItem);

        Task<SaleItem> DeleteAsync(Guid Id);
    }
}
