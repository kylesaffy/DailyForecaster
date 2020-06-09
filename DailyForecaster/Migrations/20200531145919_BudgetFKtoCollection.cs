using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class BudgetFKtoCollection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Budget_Collections_CollectionsObjCollectionsId",
                table: "Budget");

            migrationBuilder.DropIndex(
                name: "IX_Budget_CollectionsObjCollectionsId",
                table: "Budget");

            migrationBuilder.DropColumn(
                name: "CollectionsObjCollectionsId",
                table: "Budget");

            migrationBuilder.AlterColumn<string>(
                name: "CollectionId",
                table: "Budget",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Budget_CollectionId",
                table: "Budget",
                column: "CollectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Budget_Collections_CollectionId",
                table: "Budget",
                column: "CollectionId",
                principalTable: "Collections",
                principalColumn: "CollectionsId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Budget_Collections_CollectionId",
                table: "Budget");

            migrationBuilder.DropIndex(
                name: "IX_Budget_CollectionId",
                table: "Budget");

            migrationBuilder.AlterColumn<string>(
                name: "CollectionId",
                table: "Budget",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "CollectionsObjCollectionsId",
                table: "Budget",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Budget_CollectionsObjCollectionsId",
                table: "Budget",
                column: "CollectionsObjCollectionsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Budget_Collections_CollectionsObjCollectionsId",
                table: "Budget",
                column: "CollectionsObjCollectionsId",
                principalTable: "Collections",
                principalColumn: "CollectionsId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
