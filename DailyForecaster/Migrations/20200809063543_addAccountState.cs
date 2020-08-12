using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class addAccountState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountState",
                columns: table => new
                {
                    AccountStateId = table.Column<string>(nullable: false),
                    BudgetId = table.Column<string>(nullable: true),
                    AccountId = table.Column<string>(nullable: true),
                    Amount = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountState", x => x.AccountStateId);
                    table.ForeignKey(
                        name: "FK_AccountState_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountState_Budget_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budget",
                        principalColumn: "BudgetId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountState_AccountId",
                table: "AccountState",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountState_BudgetId",
                table: "AccountState",
                column: "BudgetId",
                unique: true,
                filter: "[BudgetId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountState");
        }
    }
}
