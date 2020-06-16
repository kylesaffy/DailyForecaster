using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class AddingAccountSpecifics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                table: "ManualCashFlows",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<double>(
                name: "CreditRate",
                table: "Account",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DebitRate",
                table: "Account",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "Floating",
                table: "Account",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "FloatingType",
                table: "Account",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ManualCashFlows_AccountId",
                table: "ManualCashFlows",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_ManualCashFlows_Account_AccountId",
                table: "ManualCashFlows",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ManualCashFlows_Account_AccountId",
                table: "ManualCashFlows");

            migrationBuilder.DropIndex(
                name: "IX_ManualCashFlows_AccountId",
                table: "ManualCashFlows");

            migrationBuilder.DropColumn(
                name: "CreditRate",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "DebitRate",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "Floating",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "FloatingType",
                table: "Account");

            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                table: "ManualCashFlows",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
