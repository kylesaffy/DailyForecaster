using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class expensemodels1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemisedProducts_ProductsModel_Products",
                table: "ItemisedProducts");

            migrationBuilder.DropIndex(
                name: "IX_ItemisedProducts_Products",
                table: "ItemisedProducts");

            migrationBuilder.DropColumn(
                name: "Products",
                table: "ItemisedProducts");

            migrationBuilder.DropColumn(
                name: "ProductsId",
                table: "ItemisedProducts");

            migrationBuilder.AddColumn<string>(
                name: "ExpenseModelId",
                table: "ItemisedProducts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductsModelId",
                table: "ItemisedProducts",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemisedProducts_ExpenseModelId",
                table: "ItemisedProducts",
                column: "ExpenseModelId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemisedProducts_ProductsModelId",
                table: "ItemisedProducts",
                column: "ProductsModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemisedProducts_ExpenseModels_ExpenseModelId",
                table: "ItemisedProducts",
                column: "ExpenseModelId",
                principalTable: "ExpenseModels",
                principalColumn: "ExpenseModelId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemisedProducts_ProductsModel_ProductsModelId",
                table: "ItemisedProducts",
                column: "ProductsModelId",
                principalTable: "ProductsModel",
                principalColumn: "ProductsModelId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemisedProducts_ExpenseModels_ExpenseModelId",
                table: "ItemisedProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemisedProducts_ProductsModel_ProductsModelId",
                table: "ItemisedProducts");

            migrationBuilder.DropIndex(
                name: "IX_ItemisedProducts_ExpenseModelId",
                table: "ItemisedProducts");

            migrationBuilder.DropIndex(
                name: "IX_ItemisedProducts_ProductsModelId",
                table: "ItemisedProducts");

            migrationBuilder.DropColumn(
                name: "ExpenseModelId",
                table: "ItemisedProducts");

            migrationBuilder.DropColumn(
                name: "ProductsModelId",
                table: "ItemisedProducts");

            migrationBuilder.AddColumn<string>(
                name: "Products",
                table: "ItemisedProducts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductsId",
                table: "ItemisedProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemisedProducts_Products",
                table: "ItemisedProducts",
                column: "Products");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemisedProducts_ProductsModel_Products",
                table: "ItemisedProducts",
                column: "Products",
                principalTable: "ProductsModel",
                principalColumn: "ProductsModelId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
