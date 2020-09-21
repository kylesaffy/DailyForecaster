using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class updateSplitTransactions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SplitTransactions",
                columns: table => new
                {
                    SplitTransactionsId = table.Column<string>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    CFTYpeID = table.Column<string>(nullable: true),
                    AutomatedCashFlowId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SplitTransactions", x => x.SplitTransactionsId);
                    table.ForeignKey(
                        name: "FK_SplitTransactions_AutomatedCashFlows_AutomatedCashFlowId",
                        column: x => x.AutomatedCashFlowId,
                        principalTable: "AutomatedCashFlows",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SplitTransactions_CFTypes_CFTYpeID",
                        column: x => x.CFTYpeID,
                        principalTable: "CFTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SplitTransactions_AutomatedCashFlowId",
                table: "SplitTransactions",
                column: "AutomatedCashFlowId");

            migrationBuilder.CreateIndex(
                name: "IX_SplitTransactions_CFTYpeID",
                table: "SplitTransactions",
                column: "CFTYpeID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SplitTransactions");
        }
    }
}
