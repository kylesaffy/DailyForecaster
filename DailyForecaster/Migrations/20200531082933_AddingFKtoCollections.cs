using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class AddingFKtoCollections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserCreated",
                table: "Collections",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Collections_UserCreated",
                table: "Collections",
                column: "UserCreated");

            migrationBuilder.AddForeignKey(
                name: "FK_Collections_AspNetUsers_UserCreated",
                table: "Collections",
                column: "UserCreated",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Collections_AspNetUsers_UserCreated",
                table: "Collections");

            migrationBuilder.DropIndex(
                name: "IX_Collections_UserCreated",
                table: "Collections");

            migrationBuilder.AlterColumn<string>(
                name: "UserCreated",
                table: "Collections",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

           

            
        }
    }
}
