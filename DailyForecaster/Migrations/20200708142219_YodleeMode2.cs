using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class YodleeMode2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CollectionsId",
                table: "YodleeModel",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_YodleeModel_CollectionsId",
                table: "YodleeModel",
                column: "CollectionsId");

            migrationBuilder.AddForeignKey(
                name: "FK_YodleeModel_Collections_CollectionsId",
                table: "YodleeModel",
                column: "CollectionsId",
                principalTable: "Collections",
                principalColumn: "CollectionsId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_YodleeModel_Collections_CollectionsId",
                table: "YodleeModel");

            migrationBuilder.DropIndex(
                name: "IX_YodleeModel_CollectionsId",
                table: "YodleeModel");

            migrationBuilder.DropColumn(
                name: "CollectionsId",
                table: "YodleeModel");
        }
    }
}
