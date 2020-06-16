using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class budgetUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "BudgetId",
                table: "BudgetTransactions",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetTransactions_BudgetId",
                table: "BudgetTransactions",
                column: "BudgetId");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetTransactions_Budget_BudgetId",
                table: "BudgetTransactions",
                column: "BudgetId",
                principalTable: "Budget",
                principalColumn: "BudgetId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetTransactions_Budget_BudgetId",
                table: "BudgetTransactions");

            migrationBuilder.DropIndex(
                name: "IX_BudgetTransactions_BudgetId",
                table: "BudgetTransactions");

            migrationBuilder.AlterColumn<string>(
                name: "BudgetId",
                table: "BudgetTransactions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
