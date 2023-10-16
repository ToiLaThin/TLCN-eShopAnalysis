using eShopAnalysis.CouponSaleItemAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eShopAnalysis.CouponSaleItemAPI.EntityConfigurations
{
    public class CouponEntityTypeConfiguration : IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> couponBuilder)
        {
            couponBuilder.ToTable("Coupon", "Discount");
            couponBuilder.HasKey(c => c.CouponId);
            couponBuilder.HasAlternateKey(c => c.CouponCode); //unique

            //because if we found the coupon, we want to fast check the status, if it's ended
            //no need to access the table
            couponBuilder.HasIndex(c => c.CouponCode)
                         .IncludeProperties(c => c.CouponStatus);

            couponBuilder.HasMany<CouponUser>()
                         .WithOne(cU => cU.CouponUsed) //specify navigation property, so with couponUser we can retrive all the coupon used
                         .HasPrincipalKey(c => c.CouponId)
                         .HasForeignKey(cU => cU.CouponId);

            //make sure the minOrderValueToApply Range from 0 - infinite, rewarepointRequire is also limited
            //DateEnded > DateAdded and more than 30 day

            //no coupon with same discount value and type, rewardpoint that are active at the same time to be added

            //options are validator and trigger

        }
    }
}
