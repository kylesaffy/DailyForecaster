using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class simulationAssumptionUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                table: "SimulationAssumptions",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Amount",
                table: "SimulationAssumptions",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "SimulationAssumptions");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "SimulationAssumptions");
        }
    }
}
