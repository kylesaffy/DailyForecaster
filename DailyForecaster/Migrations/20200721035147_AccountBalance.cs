using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class AccountBalance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountBalance",
                columns: table => new
                {
                    AccountBalanceId = table.Column<string>(nullable: false),
                    AccountId = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    Amount = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountBalance", x => x.AccountBalanceId);
                    table.ForeignKey(
                        name: "FK_AccountBalance_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountBalance_AccountId",
                table: "AccountBalance",
                column: "AccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountBalance");
        }
    }
}
