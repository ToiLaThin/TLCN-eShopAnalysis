using eShopAnalysis.CustomerLoyaltyProgramAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.EntityConfigurations
{
    public class RewardTransactionEntityTypeConfiguration : IEntityTypeConfiguration<RewardTransaction>
    {
        string schema = "CustomerLoyaltyProgram";
        public void Configure(EntityTypeBuilder<RewardTransaction> builder)
        {
            builder.ToTable("RewardTransaction", schema);
            builder.HasKey(rt => rt.RewardTransactionId);

            //config json column in POCO
            //builder.OwnsOne(rt => rt.Origin, o => {
            //    o.ToJson();

            //    //these prop is nullable depending on the OriginType
            //    o.Property(o => o.DiscountValue).IsRequired(false);
            //    o.Property(o => o.DiscountType).IsRequired(false);
            //    o.Property(o => o.OrderPrice).IsRequired(false);
            //});

            //config json column in jsonb
            //this have less query option, but that 's ok, because these data should be only for ETL
            builder.Property(rt => rt.Origin)
                .HasColumnType("jsonb");
        }
    }
}
