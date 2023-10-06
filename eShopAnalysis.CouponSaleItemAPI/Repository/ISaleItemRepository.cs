using eShopAnalysis.CouponSaleItemAPI.Models;

namespace eShopAnalysis.CouponSaleItemAPI.Repository
{
    public interface ISaleItemRepository
    {
        SaleItem Get(Guid Id);

        IEnumerable<SaleItem> GetAll();

        IQueryable<SaleItem> GetAllAsQueryable();

        SaleItem Add(SaleItem nSaleItem);

        SaleItem Update(SaleItem uSaleItem);

        SaleItem Delete(Guid Id);
    }
}
