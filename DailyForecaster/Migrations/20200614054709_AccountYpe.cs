using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class AccountYpe : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountTypeId",
                table: "Account",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MonthlyFee",
                table: "Account",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "AccountType",
                columns: table => new
                {
                    AccountTypeId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountType", x => x.AccountTypeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_AccountTypeId",
                table: "Account",
                column: "AccountTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_AccountType_AccountTypeId",
                table: "Account",
                column: "AccountTypeId",
                principalTable: "AccountType",
                principalColumn: "AccountTypeId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_AccountType_AccountTypeId",
                table: "Account");

            migrationBuilder.DropTable(
                name: "AccountType");

            migrationBuilder.DropIndex(
                name: "IX_Account_AccountTypeId",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "AccountTypeId",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "MonthlyFee",
                table: "Account");
        }
    }
}
