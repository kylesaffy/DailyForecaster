using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class UpdateBudgetTransactions1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetTransactions_AspNetUsers_UseId",
                table: "BudgetTransactions");

            migrationBuilder.DropIndex(
                name: "IX_BudgetTransactions_UseId",
                table: "BudgetTransactions");

            migrationBuilder.DropColumn(
                name: "UseId",
                table: "BudgetTransactions");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "BudgetTransactions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BudgetTransactions_UserId",
                table: "BudgetTransactions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetTransactions_AspNetUsers_UserId",
                table: "BudgetTransactions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetTransactions_AspNetUsers_UserId",
                table: "BudgetTransactions");

            migrationBuilder.DropIndex(
                name: "IX_BudgetTransactions_UserId",
                table: "BudgetTransactions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BudgetTransactions");

            migrationBuilder.AddColumn<string>(
                name: "UseId",
                table: "BudgetTransactions",
                type: "nvarchar(128)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BudgetTransactions_UseId",
                table: "BudgetTransactions",
                column: "UseId");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetTransactions_AspNetUsers_UseId",
                table: "BudgetTransactions",
                column: "UseId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
