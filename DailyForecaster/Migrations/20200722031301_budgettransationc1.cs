using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class budgettransationc1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Automated",
                table: "BudgetTransactions",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Automated",
                table: "BudgetTransactions");
        }
    }
}
