using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class SimUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Simulation",
                table: "Budget");

            migrationBuilder.AddColumn<bool>(
                name: "SimulationBool",
                table: "Budget",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SimulationBool",
                table: "Budget");

            migrationBuilder.AddColumn<bool>(
                name: "Simulation",
                table: "Budget",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
