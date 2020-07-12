using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class automatedCashFlows2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                table: "AutomatedCashFlows",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_AutomatedCashFlows_AccountId",
                table: "AutomatedCashFlows",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_AutomatedCashFlows_Account_AccountId",
                table: "AutomatedCashFlows",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AutomatedCashFlows_Account_AccountId",
                table: "AutomatedCashFlows");

            migrationBuilder.DropIndex(
                name: "IX_AutomatedCashFlows_AccountId",
                table: "AutomatedCashFlows");

            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                table: "AutomatedCashFlows",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
