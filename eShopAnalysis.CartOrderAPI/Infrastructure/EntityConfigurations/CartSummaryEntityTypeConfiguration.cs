using eShopAnalysis.CartOrderAPI.Domain.DomainModels.CartAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eShopAnalysis.CartOrderAPI.Infrastructure.EntityConfigurations
{
    public class CartSummaryEntityTypeConfiguration : IEntityTypeConfiguration<CartSummary>
    {
        public void Configure(EntityTypeBuilder<CartSummary> cartBuilderConfig)
        {
            cartBuilderConfig.ToTable("Cart");
            cartBuilderConfig.HasKey(c => c.Id);

            //this will caused cartItemTable to have additional column generated to be foreign key of this if not specify the navigation prop in .HasMany<CartItem>(c => c.Items)
            //see this: https://stackoverflow.com/questions/66825913/ef-core-unwanted-foreign-keys-being-added-automatically-when-i-specify-my-own
            //might be the default behavior
            cartBuilderConfig.HasMany<CartItem>(c => c.Items) //include navigation properties to explicitly configure the foreign keys => avoid extra foreign key
                             .WithOne() //no navigation prop
                             .HasPrincipalKey(c => c.Id)
                             .HasForeignKey(cI => cI.CartId)
                             .IsRequired()
                             .OnDelete(DeleteBehavior.Cascade);

            cartBuilderConfig.Property(c => c.UserId).IsRequired().ValueGeneratedNever(); //in above layer, please validate so that this not accepting the default value
            cartBuilderConfig.Property(c => c.HaveCouponApplied).HasDefaultValue(false);
            cartBuilderConfig.Property(c => c.HaveAnySaleItem).HasDefaultValue(false);
            cartBuilderConfig.Property(c => c.CouponId).HasDefaultValue(null);
            cartBuilderConfig.Property(c => c.CouponDiscountType).HasDefaultValue(null);
            cartBuilderConfig.Property(c => c.CouponDiscountAmount).HasDefaultValue(-1);
            cartBuilderConfig.Property(c => c.CouponDiscountValue).HasDefaultValue(-1);
            cartBuilderConfig.Property(c => c.TotalSaleDiscountAmount).HasDefaultValue(-1);
            cartBuilderConfig.Property(c => c.TotalPriceAfterSale).HasDefaultValue(-1);
            cartBuilderConfig.Property(c => c.TotalPriceAfterCouponApplied).HasDefaultValue(-1);
        }
    }
}
