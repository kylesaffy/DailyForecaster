using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class manualCashFlow1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.AddColumn<string>(
                name: "AutomatedCashFlowId",
                table: "ManualCashFlows",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ManualCashFlows_AutomatedCashFlowId",
                table: "ManualCashFlows",
                column: "AutomatedCashFlowId",
                unique: true,
                filter: "[AutomatedCashFlowId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ManualCashFlows_AutomatedCashFlows_AutomatedCashFlowId",
                table: "ManualCashFlows",
                column: "AutomatedCashFlowId",
                principalTable: "AutomatedCashFlows",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
         
            
            migrationBuilder.AddColumn<string>(
                name: "AutomateCashFlowId",
                table: "ManualCashFlows",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ManualCashFlows_AutomateCashFlowId",
                table: "ManualCashFlows",
                column: "AutomateCashFlowId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ManualCashFlows_AutomatedCashFlows_AutomateCashFlowId",
                table: "ManualCashFlows",
                column: "AutomateCashFlowId",
                principalTable: "AutomatedCashFlows",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
