using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class AccountsUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isDeleted",
                table: "Account",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "Account");
        }
    }
}
