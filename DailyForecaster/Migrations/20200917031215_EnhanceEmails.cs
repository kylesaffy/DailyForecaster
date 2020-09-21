using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class EnhanceEmails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Firebase",
                table: "ClickTracker",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "EmailPrefernces",
                columns: table => new
                {
                    EmailPreferencesId = table.Column<string>(nullable: false),
                    FirebaseUserId = table.Column<string>(nullable: true),
                    LastInteraction = table.Column<DateTime>(nullable: false),
                    InteractionRecord = table.Column<string>(nullable: true),
                    LoginNotification = table.Column<bool>(nullable: false),
                    DailyCommunication = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailPrefernces", x => x.EmailPreferencesId);
                    table.ForeignKey(
                        name: "FK_EmailPrefernces_FirebaseUser_FirebaseUserId",
                        column: x => x.FirebaseUserId,
                        principalTable: "FirebaseUser",
                        principalColumn: "FirebaseUserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailPrefernces_FirebaseUserId",
                table: "EmailPrefernces",
                column: "FirebaseUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailPrefernces");

            migrationBuilder.DropColumn(
                name: "Firebase",
                table: "ClickTracker");
        }
    }
}
