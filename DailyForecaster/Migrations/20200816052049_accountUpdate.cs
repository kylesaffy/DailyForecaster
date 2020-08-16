using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class accountUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SimulationId",
                table: "Account",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Account_SimulationId",
                table: "Account",
                column: "SimulationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_Simulation_SimulationId",
                table: "Account",
                column: "SimulationId",
                principalTable: "Simulation",
                principalColumn: "SimulationId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_Simulation_SimulationId",
                table: "Account");

            migrationBuilder.DropIndex(
                name: "IX_Account_SimulationId",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "SimulationId",
                table: "Account");
        }
    }
}
