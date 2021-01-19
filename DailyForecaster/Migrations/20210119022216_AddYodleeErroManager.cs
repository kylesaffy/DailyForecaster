using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class AddYodleeErroManager : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "YodleeErrorManager",
                columns: table => new
                {
                    YodleeErrorManagerId = table.Column<string>(nullable: false),
                    ErrorCode = table.Column<string>(nullable: true),
                    ErrorMessage = table.Column<string>(nullable: true),
                    ReferenceCode = table.Column<string>(nullable: true),
                    CollectionsId = table.Column<string>(nullable: true),
                    Function = table.Column<string>(nullable: true),
                    YodleeFail = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YodleeErrorManager", x => x.YodleeErrorManagerId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "YodleeErrorManager");
        }
    }
}
