using eShopAnalysis.CustomerLoyaltyProgramAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.EntityConfigurations
{
    public class UserRewardPointEntityTypeConfiguration : IEntityTypeConfiguration<UserRewardPoint>
    {
        string schema = "CustomerLoyaltyProgram";
        public void Configure(EntityTypeBuilder<UserRewardPoint> builder)
        {
            builder.ToTable("UserRewardPoint", schema);
            builder.HasKey(rt => rt.UserId);
        }
    }
}
