using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class daylies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyMotivational",
                columns: table => new
                {
                    DailyMotivationalId = table.Column<string>(nullable: false),
                    URL = table.Column<string>(nullable: true),
                    Source = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyMotivational", x => x.DailyMotivationalId);
                });

            migrationBuilder.CreateTable(
                name: "DailyTip",
                columns: table => new
                {
                    DailyTipId = table.Column<string>(nullable: false),
                    Quote = table.Column<string>(nullable: true),
                    DistributionDate = table.Column<DateTime>(nullable: false),
                    Source = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyTip", x => x.DailyTipId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyMotivational");

            migrationBuilder.DropTable(
                name: "DailyTip");
        }
    }
}
