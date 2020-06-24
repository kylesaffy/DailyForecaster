using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class AddClickTracker : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClickTracker",
                columns: table => new
                {
                    ClickTrackerId = table.Column<string>(nullable: false),
                    Location = table.Column<string>(nullable: true),
                    GET = table.Column<bool>(nullable: false),
                    POST = table.Column<bool>(nullable: false),
                    RecordDateTime = table.Column<DateTime>(nullable: false),
                    RequestData = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClickTracker", x => x.ClickTrackerId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClickTracker");
        }
    }
}
