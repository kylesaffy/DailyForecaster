using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class AccountChangeId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                table: "AccountChange",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountChange_AccountId",
                table: "AccountChange",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountChange_Account_AccountId",
                table: "AccountChange",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountChange_Account_AccountId",
                table: "AccountChange");

            migrationBuilder.DropIndex(
                name: "IX_AccountChange_AccountId",
                table: "AccountChange");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "AccountChange");
        }
    }
}
