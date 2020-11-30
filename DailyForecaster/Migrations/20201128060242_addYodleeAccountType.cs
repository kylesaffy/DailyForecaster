using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class addYodleeAccountType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YodleeString",
                table: "AccountType");

            migrationBuilder.CreateTable(
                name: "YodleeAccountType",
                columns: table => new
                {
                    YodleeAccountTypeId = table.Column<string>(nullable: false),
                    YodleeDesc = table.Column<string>(nullable: true),
                    AccountTypeId = table.Column<string>(nullable: true),
                    Container = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YodleeAccountType", x => x.YodleeAccountTypeId);
                    table.ForeignKey(
                        name: "FK_YodleeAccountType_AccountType_AccountTypeId",
                        column: x => x.AccountTypeId,
                        principalTable: "AccountType",
                        principalColumn: "AccountTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_YodleeAccountType_AccountTypeId",
                table: "YodleeAccountType",
                column: "AccountTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "YodleeAccountType");

            migrationBuilder.AddColumn<string>(
                name: "YodleeString",
                table: "AccountType",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
