using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class firebaseClasses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CFClassificationId",
                table: "SimulationAssumptions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CFTypeId",
                table: "SimulationAssumptions",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Recurring",
                table: "SimulationAssumptions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "FirebaseUser",
                columns: table => new
                {
                    FirebaseUserId = table.Column<string>(nullable: false),
                    FirebaseId = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FirebaseUser", x => x.FirebaseUserId);
                });

            migrationBuilder.CreateTable(
                name: "FirebaseLogin",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    FirebaseUserId = table.Column<string>(nullable: true),
                    DateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FirebaseLogin", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FirebaseLogin_FirebaseUser_FirebaseUserId",
                        column: x => x.FirebaseUserId,
                        principalTable: "FirebaseUser",
                        principalColumn: "FirebaseUserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SimulationAssumptions_CFClassificationId",
                table: "SimulationAssumptions",
                column: "CFClassificationId");

            migrationBuilder.CreateIndex(
                name: "IX_SimulationAssumptions_CFTypeId",
                table: "SimulationAssumptions",
                column: "CFTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FirebaseLogin_FirebaseUserId",
                table: "FirebaseLogin",
                column: "FirebaseUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SimulationAssumptions_CFClassifications_CFClassificationId",
                table: "SimulationAssumptions",
                column: "CFClassificationId",
                principalTable: "CFClassifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SimulationAssumptions_CFTypes_CFTypeId",
                table: "SimulationAssumptions",
                column: "CFTypeId",
                principalTable: "CFTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SimulationAssumptions_CFClassifications_CFClassificationId",
                table: "SimulationAssumptions");

            migrationBuilder.DropForeignKey(
                name: "FK_SimulationAssumptions_CFTypes_CFTypeId",
                table: "SimulationAssumptions");

            migrationBuilder.DropTable(
                name: "FirebaseLogin");

            migrationBuilder.DropTable(
                name: "FirebaseUser");

            migrationBuilder.DropIndex(
                name: "IX_SimulationAssumptions_CFClassificationId",
                table: "SimulationAssumptions");

            migrationBuilder.DropIndex(
                name: "IX_SimulationAssumptions_CFTypeId",
                table: "SimulationAssumptions");

            migrationBuilder.DropColumn(
                name: "CFClassificationId",
                table: "SimulationAssumptions");

            migrationBuilder.DropColumn(
                name: "CFTypeId",
                table: "SimulationAssumptions");

            migrationBuilder.DropColumn(
                name: "Recurring",
                table: "SimulationAssumptions");
        }
    }
}
