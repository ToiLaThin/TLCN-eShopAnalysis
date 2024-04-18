using eShopAnalysis.CartOrderAPI.Domain.DomainModels.CartAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace eShopAnalysis.CartOrderAPI.Infrastructure.EntityConfigurations
{
    public class CartItemEntityTypeConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> cartItemBuilderConfig)
        {
            cartItemBuilderConfig.ToTable(nameof(CartItem));
            cartItemBuilderConfig.HasKey(cI => new {
                cI.ProductId, 
                cI.ProductModelId, 
                cI.BusinessKey, 
                cI.CartId 
            });
            cartItemBuilderConfig.HasIndex(cI => new
            {
                cI.ProductId,
                cI.ProductModelId,
                cI.BusinessKey,
                cI.CartId
            }, "IndexPKCartItem").IncludeProperties()
                                 .IsUnique(true);
            //alternate key also created to be index
            //foreign key is config in CartSummaryEntityConfiguration since it have navigation prop in its model => to avoid extra foreign key

            //optional prop
            cartItemBuilderConfig.Property(cI => cI.SaleItemId).HasDefaultValue(null);
            cartItemBuilderConfig.Property(cI => cI.SaleType)
                                 .HasDefaultValue(null)
                                 .IsRequired(false);
            cartItemBuilderConfig.Property(cI => cI.SaleValue)
                                 .HasDefaultValue(-1);
            cartItemBuilderConfig.Property(cI => cI.UnitAfterSalePrice)
                                 .HasDefaultValue(-1);
            cartItemBuilderConfig.Property(cI => cI.FinalAfterSalePrice)
                                 .HasDefaultValue(-1);

            //must-have-prop
            cartItemBuilderConfig.Property(cI => cI.UnitPrice).IsRequired();
            cartItemBuilderConfig.Property(cI => cI.FinalPrice).IsRequired();

            //primary key but not generated
            cartItemBuilderConfig.Property(cI => cI.ProductId).ValueGeneratedNever();
            cartItemBuilderConfig.Property(cI => cI.ProductModelId).ValueGeneratedNever();
            cartItemBuilderConfig.Property(cI => cI.BusinessKey).ValueGeneratedNever();
            cartItemBuilderConfig.Property(cI => cI.CartId).ValueGeneratedNever();

            cartItemBuilderConfig.Property(cI => cI.ProductName).IsRequired().HasMaxLength(300);
            cartItemBuilderConfig.Property(cI => cI.SubCatalogName).IsRequired().HasMaxLength(200);
        }
    }
}
