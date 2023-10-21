using eShopAnalysis.PaymentAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eShopAnalysis.PaymentAPI.Data.EntityConfigurations
{
    public class StripeTransactionEntityTypeConfiguration : IEntityTypeConfiguration<StripeTransaction>
    {
        public void Configure(EntityTypeBuilder<StripeTransaction> stripeTransBuilder)
        {
            stripeTransBuilder.ToTable(nameof(StripeTransaction));

            stripeTransBuilder.HasKey(st => st.PaymentIntentId);
            stripeTransBuilder.HasOne<UserCustomerMapping>()
                              .WithMany()
                              .HasForeignKey(st => st.CustomerId)
                              .HasPrincipalKey(map => map.CustomerId)
                              .OnDelete(DeleteBehavior.Cascade);

            stripeTransBuilder.HasIndex(trans => trans.PaymentIntentId)
                              .IncludeProperties(trans => trans.TransactionStatus);

            stripeTransBuilder.Property(trans => trans.OrderId).IsRequired(true);
            stripeTransBuilder.Property(trans => trans.CustomerId).IsRequired(true);
            stripeTransBuilder.Property(trans => trans.TransactionStatus).HasDefaultValue(PaymentStatus.Pending);
            stripeTransBuilder.Property(trans => trans.SubTotal).IsRequired(true);
            stripeTransBuilder.Property(trans => trans.TotalDiscount).HasDefaultValue(0);
            stripeTransBuilder.Property(trans => trans.Tax).HasDefaultValue(0);
            stripeTransBuilder.Property(trans => trans.Total).HasComputedColumnSql("SubTotal - TotalDiscount + Tax", stored: true);
        }
    }
}
