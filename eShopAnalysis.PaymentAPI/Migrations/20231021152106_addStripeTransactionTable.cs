using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShopAnalysis.PaymentAPI.Migrations
{
    /// <inheritdoc />
    public partial class addStripeTransactionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StripeTransaction",
                columns: table => new
                {
                    PaymentIntentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CardId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionStatus = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    SubTotal = table.Column<double>(type: "float", nullable: false),
                    TotalDiscount = table.Column<double>(type: "float", nullable: false, defaultValue: 0.0),
                    Tax = table.Column<double>(type: "float", nullable: false, defaultValue: 0.0),
                    Total = table.Column<double>(type: "float", nullable: false, computedColumnSql: "SubTotal - TotalDiscount + Tax", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StripeTransaction", x => x.PaymentIntentId);
                    table.ForeignKey(
                        name: "FK_StripeTransaction_UserCustomerMapping_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "UserCustomerMapping",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StripeTransaction_CustomerId",
                table: "StripeTransaction",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_StripeTransaction_PaymentIntentId",
                table: "StripeTransaction",
                column: "PaymentIntentId")
                .Annotation("SqlServer:Include", new[] { "TransactionStatus" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StripeTransaction");
        }
    }
}
