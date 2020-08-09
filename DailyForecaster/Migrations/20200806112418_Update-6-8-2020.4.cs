using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class Update6820204 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_BudgetTransactions_FirebaseUserId",
                table: "BudgetTransactions",
                column: "FirebaseUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BudgetTransactions_FirebaseUserId",
                table: "BudgetTransactions");
        }
    }
}
