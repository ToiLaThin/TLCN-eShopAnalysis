using eShopAnalysis.CartOrderAPI.Domain.DomainModels.CartAggregate;
using eShopAnalysis.CartOrderAPI.Domain.DomainModels.OrderAggregate;
using eShopAnalysis.CartOrderAPI.Domain.SeedWork;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json.Linq;

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

            //orderBuilderConfig.Property(o => o.Address).IsRequired(false).HasDefaultValue(null);
            orderBuilderConfig.Property(o => o.PhoneNumber).IsRequired(false).HasDefaultValue(null);
            orderBuilderConfig.Property(o => o.Revision).HasDefaultValue(1);
            orderBuilderConfig.Property(o => o.OrdersStatus).IsRequired().HasDefaultValue(OrderStatus.CreatedDraft);
            orderBuilderConfig.Property(o => o.DateCustomerInfoConfirmed).ValueGeneratedNever().IsRequired(false);
            orderBuilderConfig.Property(o => o.DateCheckouted).ValueGeneratedNever().IsRequired(false);
            orderBuilderConfig.Property(o => o.DateStockConfirmed).ValueGeneratedNever().IsRequired(false);
            orderBuilderConfig.Property(o => o.DateRefunded).ValueGeneratedNever().IsRequired(false);
            orderBuilderConfig.Property(o => o.DateCancelled).ValueGeneratedNever().IsRequired(false);
            orderBuilderConfig.Property(o => o.DateCompleted).ValueGeneratedNever().IsRequired(false);


            //CONFIG SHADOW PROPERTY ADDRESS STORE ON ANOTHER TABLE
            //orderBuilderConfig.OwnsOne(o => o.Address, a =>
            //{
            //    // Explicit configuration of the shadow key property in the owned type 
            //    // as a workaround for a documented issue in EF Core 5: https://github.com/dotnet/efcore/issues/20740

            //    //error The property 'OrderId' on entity type 'Address' is configured to use 'SequenceHiLo' value generator, which is only intended for keys. If this was intentional, configure an alternate key on the property, otherwise call 'ValueGeneratedNever' or configure store generation for this property.
            //    a.Property<int>("OrderId")
            //    .UseHiLo("orderseq");
            //    a.WithOwner();

            //});
            orderBuilderConfig.OwnsOne(nameof(Address), o => o.Address, a => {
                //this is required now, may related to newer version of EF Core to store Address to a separate table
                a.ToTable(nameof(Address));
                a.HasKey("OrderId");
                //SQL Server sequences cannot be used to generate values for the property 'OrderId' on entity type 'Address (Address)' because the property type is 'Guid'.Sequences can only be used with integer properties.
                //a.Property<Guid>("OrderId").UseHiLo("orderseq");
                a.WithOwner().HasForeignKey("OrderId").HasPrincipalKey(o => o.Id);

            });
        }
    }
}
