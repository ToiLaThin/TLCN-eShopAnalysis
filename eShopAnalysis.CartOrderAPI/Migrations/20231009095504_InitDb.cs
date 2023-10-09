using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShopAnalysis.CartOrderAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cart",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HaveCouponApplied = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    HaveAnySaleItem = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CouponDiscountType = table.Column<int>(type: "int", nullable: true),
                    CouponDiscountAmount = table.Column<double>(type: "float", nullable: false, defaultValue: -1.0),
                    CouponDiscountValue = table.Column<double>(type: "float", nullable: false, defaultValue: -1.0),
                    TotalSaleDiscountAmount = table.Column<double>(type: "float", nullable: false, defaultValue: -1.0),
                    TotalPriceOriginal = table.Column<double>(type: "float", nullable: false),
                    TotalPriceAfterSale = table.Column<double>(type: "float", nullable: false, defaultValue: -1.0),
                    TotalPriceAfterCouponApplied = table.Column<double>(type: "float", nullable: false, defaultValue: -1.0),
                    TotalPriceFinal = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cart", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CartItem",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessKey = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SaleItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsOnSale = table.Column<bool>(type: "bit", nullable: false),
                    SaleType = table.Column<int>(type: "int", nullable: true),
                    SaleValue = table.Column<double>(type: "float", nullable: false, defaultValue: -1.0),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<double>(type: "float", nullable: false),
                    FinalPrice = table.Column<double>(type: "float", nullable: false),
                    UnitAfterSalePrice = table.Column<double>(type: "float", nullable: false, defaultValue: -1.0),
                    FinalAfterSalePrice = table.Column<double>(type: "float", nullable: false, defaultValue: -1.0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItem", x => new { x.ProductId, x.ProductModelId, x.BusinessKey, x.CartId });
                    table.ForeignKey(
                        name: "FK_CartItem_Cart_CartId",
                        column: x => x.CartId,
                        principalTable: "Cart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessKey = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateCheckouted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateConfirmed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateRefunded = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateCompleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Revision = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Cart_CartId",
                        column: x => x.CartId,
                        principalTable: "Cart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IndexPKCartItem",
                table: "CartItem",
                columns: new[] { "ProductId", "ProductModelId", "BusinessKey", "CartId" },
                unique: true)
                .Annotation("SqlServer:Include", new string[0]);

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_CartId",
                table: "CartItem",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CartId",
                table: "Orders",
                column: "CartId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItem");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Cart");
        }
    }
}
