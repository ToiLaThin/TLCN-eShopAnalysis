using eShopAnalysis.CouponSaleItemAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eShopAnalysis.CouponSaleItemAPI.EntityConfigurations
{
    public class CouponUserEntityTypeConfiguration : IEntityTypeConfiguration<CouponUser>
    {
        public void Configure(EntityTypeBuilder<CouponUser> couponUserBuilder)
        {
            couponUserBuilder.ToTable("CouponUser", "Discount");
            couponUserBuilder.HasKey(cU => new { cU.CouponId, cU.UserId });
            couponUserBuilder.HasIndex(cU => cU.UserId).IncludeProperties(c => new { c.CouponId }); //fast retrieve coupon used by user
        }
    }
}
