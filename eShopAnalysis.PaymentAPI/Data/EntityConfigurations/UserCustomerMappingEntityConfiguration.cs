using eShopAnalysis.PaymentAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eShopAnalysis.PaymentAPI.Data.EntityConfigurations
{
    public class UserCustomerMappingEntityConfiguration : IEntityTypeConfiguration<UserCustomerMapping>
    {
        public void Configure(EntityTypeBuilder<UserCustomerMapping> mappingBuilder)
        {
            mappingBuilder.ToTable(nameof(UserCustomerMapping));            
            mappingBuilder.HasKey(map => new { map.UserId, map.CustomerId });

            //enforce unique constraint on both UserId and CustomerId?? or in the has key it is already unique
            //for fast searching
            mappingBuilder.HasIndex(map => map.UserId)
                          .IncludeProperties(map => map.CustomerId)
                          .IsUnique();

            mappingBuilder.HasIndex(map => map.CustomerId)
                          .IncludeProperties(map => map.UserId)
                          .IsUnique();
        }
    }
}
