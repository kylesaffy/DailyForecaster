using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class budgets1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Simulation",
                table: "Budget",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Simulation",
                table: "Budget");
        }
    }
}
