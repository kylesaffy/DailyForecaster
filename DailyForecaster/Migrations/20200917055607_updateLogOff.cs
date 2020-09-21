using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class updateLogOff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailRecords",
                columns: table => new
                {
                    EmailRecordsId = table.Column<string>(nullable: false),
                    InteractionDate = table.Column<DateTime>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    FirebaseUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailRecords", x => x.EmailRecordsId);
                });

            migrationBuilder.CreateTable(
                name: "LogoffModel",
                columns: table => new
                {
                    LogoffModelId = table.Column<string>(nullable: false),
                    FirebaseUserId = table.Column<string>(nullable: true),
                    DateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogoffModel", x => x.LogoffModelId);
                    table.ForeignKey(
                        name: "FK_LogoffModel_FirebaseUser_FirebaseUserId",
                        column: x => x.FirebaseUserId,
                        principalTable: "FirebaseUser",
                        principalColumn: "FirebaseUserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LogoffModel_FirebaseUserId",
                table: "LogoffModel",
                column: "FirebaseUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailRecords");

            migrationBuilder.DropTable(
                name: "LogoffModel");
        }
    }
}
