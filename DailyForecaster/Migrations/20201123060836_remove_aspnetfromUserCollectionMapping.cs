using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class remove_aspnetfromUserCollectionMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCollectionMapping_AspNetUsers_Id",
                table: "UserCollectionMapping");

            migrationBuilder.DropIndex(
                name: "IX_UserCollectionMapping_Id",
                table: "UserCollectionMapping");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserCollectionMapping");

            migrationBuilder.AddColumn<string>(
                name: "AspNetUsersId",
                table: "UserCollectionMapping",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserCollectionMapping_AspNetUsersId",
                table: "UserCollectionMapping",
                column: "AspNetUsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCollectionMapping_AspNetUsers_AspNetUsersId",
                table: "UserCollectionMapping",
                column: "AspNetUsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCollectionMapping_AspNetUsers_AspNetUsersId",
                table: "UserCollectionMapping");

            migrationBuilder.DropIndex(
                name: "IX_UserCollectionMapping_AspNetUsersId",
                table: "UserCollectionMapping");

            migrationBuilder.DropColumn(
                name: "AspNetUsersId",
                table: "UserCollectionMapping");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "UserCollectionMapping",
                type: "nvarchar(128)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_UserCollectionMapping_Id",
                table: "UserCollectionMapping",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCollectionMapping_AspNetUsers_Id",
                table: "UserCollectionMapping",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
