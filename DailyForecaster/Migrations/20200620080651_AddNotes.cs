using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class AddNotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    NotesId = table.Column<string>(nullable: false),
                    BudgetTransactionId = table.Column<string>(nullable: false),
                    Body = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notes", x => x.NotesId);
                    table.ForeignKey(
                        name: "FK_Notes_BudgetTransactions_BudgetTransactionId",
                        column: x => x.BudgetTransactionId,
                        principalTable: "BudgetTransactions",
                        principalColumn: "BudgetTransactionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notes_BudgetTransactionId",
                table: "Notes",
                column: "BudgetTransactionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notes");
        }
    }
}
