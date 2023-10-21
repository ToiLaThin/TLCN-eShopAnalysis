using eShopAnalysis.PaymentAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eShopAnalysis.PaymentAPI.Data.EntityConfigurations
{
    public class MomoTransactionEntityTypeConfiguration : IEntityTypeConfiguration<MomoTransaction>
    {
        public void Configure(EntityTypeBuilder<MomoTransaction> momoTransBuilder)
        {
            momoTransBuilder.ToTable(name: nameof(MomoTransaction));

            momoTransBuilder.HasKey(trans => trans.TransactionId);
            momoTransBuilder.HasOne<UserCustomerMapping>()
                            .WithMany()
                            .HasForeignKey(trans => trans.CustomerId)
                            .HasPrincipalKey(map => map.CustomerId)
                            .OnDelete(DeleteBehavior.Cascade);

            momoTransBuilder.HasIndex(trans => trans.TransactionId).IncludeProperties(trans => trans.TransactionStatus);

            momoTransBuilder.Property(trans => trans.CustomerId).IsRequired(true);
            momoTransBuilder.Property(trans => trans.TransactionStatus).HasDefaultValue(PaymentStatus.Pending);
            momoTransBuilder.Property(trans => trans.SubTotal).IsRequired(true);
            momoTransBuilder.Property(trans => trans.TotalDiscount).HasDefaultValue(0);
            momoTransBuilder.Property(trans => trans.Tax).HasDefaultValue(0);
            momoTransBuilder.Property(trans => trans.Total).HasComputedColumnSql("SubTotal - TotalDiscount + Tax", stored: true);
        }
    }
}
