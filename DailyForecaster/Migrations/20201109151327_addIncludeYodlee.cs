using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class addIncludeYodlee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IncludeYodlee",
                columns: table => new
                {
                    IncludeYodleeId = table.Column<string>(nullable: false),
                    CollectionsId = table.Column<string>(nullable: true),
                    Included = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncludeYodlee", x => x.IncludeYodleeId);
                    table.ForeignKey(
                        name: "FK_IncludeYodlee_Collections_CollectionsId",
                        column: x => x.CollectionsId,
                        principalTable: "Collections",
                        principalColumn: "CollectionsId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncludeYodlee_CollectionsId",
                table: "IncludeYodlee",
                column: "CollectionsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncludeYodlee");
        }
    }
}
