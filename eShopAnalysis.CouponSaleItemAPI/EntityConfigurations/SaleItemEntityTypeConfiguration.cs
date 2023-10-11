using eShopAnalysis.CouponSaleItemAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eShopAnalysis.CouponSaleItemAPI.EntityConfigurations
{
    public class SaleItemEntityTypeConfiguration : IEntityTypeConfiguration<SaleItem>
    {
        public void Configure(EntityTypeBuilder<SaleItem> saleItemBuilder)
        {
            saleItemBuilder.ToTable("SaleItem", "Discount");
            saleItemBuilder.HasKey(s => s.SaleItemId);
            saleItemBuilder.HasIndex(c => new { c.ProductId, c.ProductModelId, c.BusinessKey}).IncludeProperties(s => s.SaleItemStatus);

            //there can only one active sale item with the same pId, pMId, BK
            //sale item that add must have product model be active now and is the latest version
        }   //https://stackoverflow.com/questions/1127122/should-data-validation-be-done-at-the-database-level
            
        //since only one {pId, pMId, BK } is active at one time, discount value, type and rewwared point can be the same
    }
}
