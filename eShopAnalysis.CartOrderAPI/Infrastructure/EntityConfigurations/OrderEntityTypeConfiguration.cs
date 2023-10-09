using eShopAnalysis.CartOrderAPI.Domain.DomainModels.CartAggregate;
using eShopAnalysis.CartOrderAPI.Domain.DomainModels.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eShopAnalysis.CartOrderAPI.Infrastructure.EntityConfigurations
{
    public class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> orderBuilderConfig)
        {
            orderBuilderConfig.HasKey(o => o.Id);

            orderBuilderConfig.HasOne<CartSummary>(c => c.Cart)
                              .WithOne()
                              .HasForeignKey<Order>(o => o.CartId);

            orderBuilderConfig.Property(o => o.Address).IsRequired();

            orderBuilderConfig.Property(o => o.Revision).HasDefaultValue(1);
            orderBuilderConfig.Property(o => o.DateRefunded).ValueGeneratedNever().IsRequired(false);
            orderBuilderConfig.Property(o => o.DateConfirmed).ValueGeneratedNever().IsRequired(false);
            orderBuilderConfig.Property(o => o.DateCompleted).ValueGeneratedNever().IsRequired(false);
            orderBuilderConfig.Property(o => o.DateCheckouted).ValueGeneratedNever().IsRequired(false);            
        }
    }
}
