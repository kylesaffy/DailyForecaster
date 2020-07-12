using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class Account1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountIdentifier",
                table: "Account",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YodleeId",
                table: "Account",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountIdentifier",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "YodleeId",
                table: "Account");
        }
    }
}
