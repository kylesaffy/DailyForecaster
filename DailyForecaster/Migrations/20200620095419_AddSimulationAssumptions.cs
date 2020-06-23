using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class AddSimulationAssumptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SimulationAssumptionsId",
                table: "Simualtion",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SimulationAssumptions",
                columns: table => new
                {
                    SimulationAssumptionsId = table.Column<string>(nullable: false),
                    NumberOfMonths = table.Column<int>(nullable: false),
                    Bonus = table.Column<bool>(nullable: false),
                    BonusMonth = table.Column<int>(nullable: false),
                    BonusAmount = table.Column<double>(nullable: false),
                    Increase = table.Column<bool>(nullable: false),
                    IncreaseMonth = table.Column<int>(nullable: false),
                    IncreasePercentage = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimulationAssumptions", x => x.SimulationAssumptionsId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Simualtion_SimulationAssumptionsId",
                table: "Simualtion",
                column: "SimulationAssumptionsId",
                unique: true,
                filter: "[SimulationAssumptionsId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Simualtion_SimulationAssumptions_SimulationAssumptionsId",
                table: "Simualtion",
                column: "SimulationAssumptionsId",
                principalTable: "SimulationAssumptions",
                principalColumn: "SimulationAssumptionsId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Simualtion_SimulationAssumptions_SimulationAssumptionsId",
                table: "Simualtion");

            migrationBuilder.DropTable(
                name: "SimulationAssumptions");

            migrationBuilder.DropIndex(
                name: "IX_Simualtion_SimulationAssumptionsId",
                table: "Simualtion");

            migrationBuilder.DropColumn(
                name: "SimulationAssumptionsId",
                table: "Simualtion");
        }
    }
}
