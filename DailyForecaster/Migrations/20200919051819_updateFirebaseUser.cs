using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class updateFirebaseUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "FirebaseUser",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "FirebaseUser",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "FirebaseUser");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "FirebaseUser");
        }
    }
}
