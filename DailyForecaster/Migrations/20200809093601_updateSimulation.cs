using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class updateSimulation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Budget_Simualtion_SimulationId",
                table: "Budget");

            migrationBuilder.DropForeignKey(
                name: "FK_Simualtion_Collections_CollectionsId",
                table: "Simualtion");

            migrationBuilder.DropForeignKey(
                name: "FK_Simualtion_SimulationAssumptions_SimulationAssumptionsId",
                table: "Simualtion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Simualtion",
                table: "Simualtion");

            migrationBuilder.RenameTable(
                name: "Simualtion",
                newName: "Simulation");

            migrationBuilder.RenameIndex(
                name: "IX_Simualtion_SimulationAssumptionsId",
                table: "Simulation",
                newName: "IX_Simulation_SimulationAssumptionsId");

            migrationBuilder.RenameIndex(
                name: "IX_Simualtion_CollectionsId",
                table: "Simulation",
                newName: "IX_Simulation_CollectionsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Simulation",
                table: "Simulation",
                column: "SimulationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Budget_Simulation_SimulationId",
                table: "Budget",
                column: "SimulationId",
                principalTable: "Simulation",
                principalColumn: "SimulationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Simulation_Collections_CollectionsId",
                table: "Simulation",
                column: "CollectionsId",
                principalTable: "Collections",
                principalColumn: "CollectionsId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Simulation_SimulationAssumptions_SimulationAssumptionsId",
                table: "Simulation",
                column: "SimulationAssumptionsId",
                principalTable: "SimulationAssumptions",
                principalColumn: "SimulationAssumptionsId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Budget_Simulation_SimulationId",
                table: "Budget");

            migrationBuilder.DropForeignKey(
                name: "FK_Simulation_Collections_CollectionsId",
                table: "Simulation");

            migrationBuilder.DropForeignKey(
                name: "FK_Simulation_SimulationAssumptions_SimulationAssumptionsId",
                table: "Simulation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Simulation",
                table: "Simulation");

            migrationBuilder.RenameTable(
                name: "Simulation",
                newName: "Simualtion");

            migrationBuilder.RenameIndex(
                name: "IX_Simulation_SimulationAssumptionsId",
                table: "Simualtion",
                newName: "IX_Simualtion_SimulationAssumptionsId");

            migrationBuilder.RenameIndex(
                name: "IX_Simulation_CollectionsId",
                table: "Simualtion",
                newName: "IX_Simualtion_CollectionsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Simualtion",
                table: "Simualtion",
                column: "SimulationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Budget_Simualtion_SimulationId",
                table: "Budget",
                column: "SimulationId",
                principalTable: "Simualtion",
                principalColumn: "SimulationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Simualtion_Collections_CollectionsId",
                table: "Simualtion",
                column: "CollectionsId",
                principalTable: "Collections",
                principalColumn: "CollectionsId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Simualtion_SimulationAssumptions_SimulationAssumptionsId",
                table: "Simualtion",
                column: "SimulationAssumptionsId",
                principalTable: "SimulationAssumptions",
                principalColumn: "SimulationAssumptionsId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
