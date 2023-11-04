using eShopAnalysis.CouponSaleItemAPI.Models;

namespace eShopAnalysis.CouponSaleItemAPI.Repository
{
    public interface ICouponUserRepository
    {
        CouponUser Add(CouponUser couponUser);
        CouponUser Delete(Guid couponId, Guid userId);
        CouponUser Get(Guid couponId, Guid userId);

        Task<CouponUser> AddAsync(CouponUser couponUser);
        Task<CouponUser> DeleteAsync(Guid couponId, Guid userId);
        Task<CouponUser> GetAsync(Guid couponId, Guid userId);

        IQueryable<CouponUser> GetAsQueryable();
        IQueryable<CouponUser> GetAsQueryableIncludedCouponUsed();
    }
}