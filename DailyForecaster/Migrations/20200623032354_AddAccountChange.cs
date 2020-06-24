using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class AddAccountChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountChange",
                columns: table => new
                {
                    AccountChangeId = table.Column<string>(nullable: false),
                    UpdatedBalance = table.Column<double>(nullable: false),
                    DateAdded = table.Column<DateTime>(nullable: false),
                    ManualCashFlowId = table.Column<string>(nullable: true),
                    AutomatedCashFlowId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountChange", x => x.AccountChangeId);
                    table.ForeignKey(
                        name: "FK_AccountChange_AutomatedCashFlows_AutomatedCashFlowId",
                        column: x => x.AutomatedCashFlowId,
                        principalTable: "AutomatedCashFlows",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountChange_ManualCashFlows_ManualCashFlowId",
                        column: x => x.ManualCashFlowId,
                        principalTable: "ManualCashFlows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountChange_AutomatedCashFlowId",
                table: "AccountChange",
                column: "AutomatedCashFlowId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountChange_ManualCashFlowId",
                table: "AccountChange",
                column: "ManualCashFlowId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountChange");
        }
    }
}
