using System;
using Microsoft.EntityFrameworkCore.Migrations;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Models;

#nullable disable

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgresDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "CustomerLoyaltyProgram");

            migrationBuilder.CreateTable(
                name: "RewardTransaction",
                schema: "CustomerLoyaltyProgram",
                columns: table => new
                {
                    RewardTransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PointTransition = table.Column<int>(type: "integer", nullable: false),
                    DateTransition = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PointBeforeTransaction = table.Column<int>(type: "integer", nullable: false),
                    PointAfterTransaction = table.Column<int>(type: "integer", nullable: false),
                    Origin = table.Column<OriginJson>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RewardTransaction", x => x.RewardTransactionId);
                });

            migrationBuilder.CreateTable(
                name: "UserRewardPoint",
                schema: "CustomerLoyaltyProgram",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RewardPoint = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRewardPoint", x => x.UserId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RewardTransaction",
                schema: "CustomerLoyaltyProgram");

            migrationBuilder.DropTable(
                name: "UserRewardPoint",
                schema: "CustomerLoyaltyProgram");
        }
    }
}
