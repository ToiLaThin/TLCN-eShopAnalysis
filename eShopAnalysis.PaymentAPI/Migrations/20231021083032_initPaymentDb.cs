using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShopAnalysis.PaymentAPI.Migrations
{
    /// <inheritdoc />
    public partial class initPaymentDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserCustomerMapping",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCustomerMapping", x => new { x.UserId, x.CustomerId });
                    table.UniqueConstraint("AK_UserCustomerMapping_CustomerId", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "MomoTransaction",
                columns: table => new
                {
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TransactionStatus = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    SubTotal = table.Column<double>(type: "float", nullable: false),
                    TotalDiscount = table.Column<double>(type: "float", nullable: false, defaultValue: 0.0),
                    Tax = table.Column<double>(type: "float", nullable: false, defaultValue: 0.0),
                    Total = table.Column<double>(type: "float", nullable: false, computedColumnSql: "SubTotal - TotalDiscount + Tax", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MomoTransaction", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_MomoTransaction_UserCustomerMapping_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "UserCustomerMapping",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MomoTransaction_CustomerId",
                table: "MomoTransaction",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_MomoTransaction_TransactionId",
                table: "MomoTransaction",
                column: "TransactionId")
                .Annotation("SqlServer:Include", new[] { "TransactionStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_UserCustomerMapping_CustomerId",
                table: "UserCustomerMapping",
                column: "CustomerId",
                unique: true)
                .Annotation("SqlServer:Include", new[] { "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserCustomerMapping_UserId",
                table: "UserCustomerMapping",
                column: "UserId",
                unique: true)
                .Annotation("SqlServer:Include", new[] { "CustomerId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MomoTransaction");

            migrationBuilder.DropTable(
                name: "UserCustomerMapping");
        }
    }
}
