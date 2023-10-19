using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShopAnalysis.CartOrderAPI.Migrations
{
    /// <inheritdoc />
    public partial class addCustomerInfoToOrderModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "DateCreated",
                table: "Orders",
                newName: "DateCreatedDraft");

            migrationBuilder.RenameColumn(
                name: "DateConfirmed",
                table: "Orders",
                newName: "DateStockConfirmed");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCancelled",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCustomerInfoConfirmed",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrdersStatus",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CityOrProvinceOrPlace = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DistrictOrLocality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullAddressName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Address_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropColumn(
                name: "DateCancelled",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DateCustomerInfoConfirmed",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrdersStatus",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "DateStockConfirmed",
                table: "Orders",
                newName: "DateConfirmed");

            migrationBuilder.RenameColumn(
                name: "DateCreatedDraft",
                table: "Orders",
                newName: "DateCreated");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
