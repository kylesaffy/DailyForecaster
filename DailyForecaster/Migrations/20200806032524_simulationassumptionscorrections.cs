using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class simulationassumptionscorrections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SimulationAssumptions_Collections_CollectionsId",
                table: "SimulationAssumptions");

            migrationBuilder.DropIndex(
                name: "IX_SimulationAssumptions_CollectionsId",
                table: "SimulationAssumptions");

            migrationBuilder.DropColumn(
                name: "CollectionsId",
                table: "SimulationAssumptions");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "SimulationAssumptions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "SimulationAssumptions");

            migrationBuilder.AddColumn<string>(
                name: "CollectionsId",
                table: "SimulationAssumptions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SimulationAssumptions_CollectionsId",
                table: "SimulationAssumptions",
                column: "CollectionsId");

            migrationBuilder.AddForeignKey(
                name: "FK_SimulationAssumptions_Collections_CollectionsId",
                table: "SimulationAssumptions",
                column: "CollectionsId",
                principalTable: "Collections",
                principalColumn: "CollectionsId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
