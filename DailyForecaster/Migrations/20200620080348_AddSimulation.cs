using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class AddSimulation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SimulationId",
                table: "Budget",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Simualtion",
                columns: table => new
                {
                    SimulationId = table.Column<string>(nullable: false),
                    SimulationName = table.Column<string>(nullable: false),
                    CollectionsId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Simualtion", x => x.SimulationId);
                    table.ForeignKey(
                        name: "FK_Simualtion_Collections_CollectionsId",
                        column: x => x.CollectionsId,
                        principalTable: "Collections",
                        principalColumn: "CollectionsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Budget_SimulationId",
                table: "Budget",
                column: "SimulationId");

            migrationBuilder.CreateIndex(
                name: "IX_Simualtion_CollectionsId",
                table: "Simualtion",
                column: "CollectionsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Budget_Simualtion_SimulationId",
                table: "Budget",
                column: "SimulationId",
                principalTable: "Simualtion",
                principalColumn: "SimulationId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Budget_Simualtion_SimulationId",
                table: "Budget");

            migrationBuilder.DropTable(
                name: "Simualtion");

            migrationBuilder.DropIndex(
                name: "IX_Budget_SimulationId",
                table: "Budget");

            migrationBuilder.DropColumn(
                name: "SimulationId",
                table: "Budget");
        }
    }
}
