using eShopAnalysis.CouponSaleItemAPI.Models;

namespace eShopAnalysis.CouponSaleItemAPI.Repository
{
    public interface ICouponUserRepository
    {
        CouponUser Add(CouponUser couponUser);
        CouponUser Delete(Guid couponId, Guid userId);
        CouponUser Get(Guid couponId, Guid userId);
        IEnumerable<CouponUser> GetAll();
    }
}