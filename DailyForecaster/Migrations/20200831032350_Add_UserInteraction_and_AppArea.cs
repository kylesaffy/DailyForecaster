using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class Add_UserInteraction_and_AppArea : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppAreas",
                columns: table => new
                {
                    AppAreasId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppAreas", x => x.AppAreasId);
                });

            migrationBuilder.CreateTable(
                name: "UserInteraction",
                columns: table => new
                {
                    UserInteractionId = table.Column<string>(nullable: false),
                    AppAreasId = table.Column<string>(nullable: true),
                    FirebaseUserId = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    Count = table.Column<int>(nullable: false),
                    AreaObejctId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInteraction", x => x.UserInteractionId);
                    table.ForeignKey(
                        name: "FK_UserInteraction_AppAreas_AppAreasId",
                        column: x => x.AppAreasId,
                        principalTable: "AppAreas",
                        principalColumn: "AppAreasId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserInteraction_FirebaseUser_FirebaseUserId",
                        column: x => x.FirebaseUserId,
                        principalTable: "FirebaseUser",
                        principalColumn: "FirebaseUserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserInteraction_AppAreasId",
                table: "UserInteraction",
                column: "AppAreasId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInteraction_FirebaseUserId",
                table: "UserInteraction",
                column: "FirebaseUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserInteraction");

            migrationBuilder.DropTable(
                name: "AppAreas");
        }
    }
}
