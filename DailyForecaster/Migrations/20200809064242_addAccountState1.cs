using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class addAccountState1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AccountState_BudgetId",
                table: "AccountState");

            migrationBuilder.CreateIndex(
                name: "IX_AccountState_BudgetId",
                table: "AccountState",
                column: "BudgetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AccountState_BudgetId",
                table: "AccountState");

            migrationBuilder.CreateIndex(
                name: "IX_AccountState_BudgetId",
                table: "AccountState",
                column: "BudgetId",
                unique: true,
                filter: "[BudgetId] IS NOT NULL");
        }
    }
}
