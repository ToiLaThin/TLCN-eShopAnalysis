using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShopAnalysis.CouponSaleItemAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitDbPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Discount");

            migrationBuilder.CreateTable(
                name: "Coupon",
                schema: "Discount",
                columns: table => new
                {
                    CouponId = table.Column<Guid>(type: "uuid", nullable: false),
                    CouponCode = table.Column<string>(type: "text", nullable: false),
                    DiscountType = table.Column<int>(type: "integer", nullable: false),
                    DiscountValue = table.Column<double>(type: "double precision", nullable: false),
                    MinOrderValueToApply = table.Column<double>(type: "double precision", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateEnded = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CouponStatus = table.Column<int>(type: "integer", nullable: false),
                    RewardPointRequire = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupon", x => x.CouponId);
                    table.UniqueConstraint("AK_Coupon_CouponCode", x => x.CouponCode);
                });

            migrationBuilder.CreateTable(
                name: "SaleItem",
                schema: "Discount",
                columns: table => new
                {
                    SaleItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessKey = table.Column<Guid>(type: "uuid", nullable: false),
                    DiscountType = table.Column<int>(type: "integer", nullable: false),
                    DiscountValue = table.Column<double>(type: "double precision", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateEnded = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SaleItemStatus = table.Column<int>(type: "integer", nullable: false),
                    RewardPointRequire = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleItem", x => x.SaleItemId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Coupon_CouponCode",
                schema: "Discount",
                table: "Coupon",
                column: "CouponCode")
                .Annotation("Npgsql:IndexInclude", new[] { "CouponStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_SaleItem_ProductId_ProductModelId_BusinessKey",
                schema: "Discount",
                table: "SaleItem",
                columns: new[] { "ProductId", "ProductModelId", "BusinessKey" })
                .Annotation("Npgsql:IndexInclude", new[] { "SaleItemStatus" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coupon",
                schema: "Discount");

            migrationBuilder.DropTable(
                name: "SaleItem",
                schema: "Discount");
        }
    }
}
