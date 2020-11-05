using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class updateproductModelClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CFTypeId",
                table: "ProductClassModel",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductClassModel_CFTypeId",
                table: "ProductClassModel",
                column: "CFTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductClassModel_CFTypes_CFTypeId",
                table: "ProductClassModel",
                column: "CFTypeId",
                principalTable: "CFTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductClassModel_CFTypes_CFTypeId",
                table: "ProductClassModel");

            migrationBuilder.DropIndex(
                name: "IX_ProductClassModel_CFTypeId",
                table: "ProductClassModel");

            migrationBuilder.DropColumn(
                name: "CFTypeId",
                table: "ProductClassModel");
        }
    }
}
