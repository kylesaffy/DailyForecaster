using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class newSimulationsStructures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bonus",
                table: "SimulationAssumptions");

            migrationBuilder.DropColumn(
                name: "BonusAmount",
                table: "SimulationAssumptions");

            migrationBuilder.DropColumn(
                name: "BonusMonth",
                table: "SimulationAssumptions");

            migrationBuilder.DropColumn(
                name: "Increase",
                table: "SimulationAssumptions");

            migrationBuilder.DropColumn(
                name: "IncreaseMonth",
                table: "SimulationAssumptions");

            migrationBuilder.DropColumn(
                name: "IncreasePercentage",
                table: "SimulationAssumptions");

            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                table: "BudgetTransactions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LineId",
                table: "BudgetTransactions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BonusModel",
                columns: table => new
                {
                    BonusModelId = table.Column<string>(nullable: false),
                    SimulationAssumptionsId = table.Column<string>(nullable: true),
                    Bonus = table.Column<bool>(nullable: false),
                    BonusMonth = table.Column<int>(nullable: false),
                    BonusAmount = table.Column<double>(nullable: false),
                    LineId = table.Column<int>(nullable: false),
                    BudgetTransactionId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BonusModel", x => x.BonusModelId);
                    table.ForeignKey(
                        name: "FK_BonusModel_BudgetTransactions_BudgetTransactionId",
                        column: x => x.BudgetTransactionId,
                        principalTable: "BudgetTransactions",
                        principalColumn: "BudgetTransactionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BonusModel_SimulationAssumptions_SimulationAssumptionsId",
                        column: x => x.SimulationAssumptionsId,
                        principalTable: "SimulationAssumptions",
                        principalColumn: "SimulationAssumptionsId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IncreaseModel",
                columns: table => new
                {
                    IncreaseModelId = table.Column<string>(nullable: false),
                    SimulationAssumptionsId = table.Column<string>(nullable: true),
                    Increase = table.Column<bool>(nullable: false),
                    IncreaseMonth = table.Column<int>(nullable: false),
                    IncreasePercentage = table.Column<double>(nullable: false),
                    LineId = table.Column<int>(nullable: false),
                    BudgetTransactionId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncreaseModel", x => x.IncreaseModelId);
                    table.ForeignKey(
                        name: "FK_IncreaseModel_BudgetTransactions_BudgetTransactionId",
                        column: x => x.BudgetTransactionId,
                        principalTable: "BudgetTransactions",
                        principalColumn: "BudgetTransactionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncreaseModel_SimulationAssumptions_SimulationAssumptionsId",
                        column: x => x.SimulationAssumptionsId,
                        principalTable: "SimulationAssumptions",
                        principalColumn: "SimulationAssumptionsId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetTransactions_AccountId",
                table: "BudgetTransactions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BonusModel_BudgetTransactionId",
                table: "BonusModel",
                column: "BudgetTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_BonusModel_SimulationAssumptionsId",
                table: "BonusModel",
                column: "SimulationAssumptionsId");

            migrationBuilder.CreateIndex(
                name: "IX_IncreaseModel_BudgetTransactionId",
                table: "IncreaseModel",
                column: "BudgetTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_IncreaseModel_SimulationAssumptionsId",
                table: "IncreaseModel",
                column: "SimulationAssumptionsId");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetTransactions_Account_AccountId",
                table: "BudgetTransactions",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetTransactions_Account_AccountId",
                table: "BudgetTransactions");

            migrationBuilder.DropTable(
                name: "BonusModel");

            migrationBuilder.DropTable(
                name: "IncreaseModel");

            migrationBuilder.DropIndex(
                name: "IX_BudgetTransactions_AccountId",
                table: "BudgetTransactions");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "BudgetTransactions");

            migrationBuilder.DropColumn(
                name: "LineId",
                table: "BudgetTransactions");

            migrationBuilder.AddColumn<bool>(
                name: "Bonus",
                table: "SimulationAssumptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "BonusAmount",
                table: "SimulationAssumptions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "BonusMonth",
                table: "SimulationAssumptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Increase",
                table: "SimulationAssumptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "IncreaseMonth",
                table: "SimulationAssumptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "IncreasePercentage",
                table: "SimulationAssumptions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
