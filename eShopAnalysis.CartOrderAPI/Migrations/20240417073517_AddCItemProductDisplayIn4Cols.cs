using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShopAnalysis.CartOrderAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddCItemProductDisplayIn4Cols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductImage",
                table: "CartItem",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "CartItem",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubCatalogName",
                table: "CartItem",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductImage",
                table: "CartItem");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "CartItem");

            migrationBuilder.DropColumn(
                name: "SubCatalogName",
                table: "CartItem");
        }
    }
}
