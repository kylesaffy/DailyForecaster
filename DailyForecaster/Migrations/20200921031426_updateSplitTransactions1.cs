using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class updateSplitTransactions1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
   //         migrationBuilder.DropForeignKey(
   //             name: "FK_SplitTransactions_CFTypes_CFTYpeID",
   //             table: "SplitTransactions");

   //         migrationBuilder.DropIndex(
   //             name: "IX_SplitTransactions_CFTYpeID",
   //             table: "SplitTransactions");

   //         migrationBuilder.RenameColumn(
   //             name: "CFTYpeID",
   //             table: "SplitTransactions",
   //             newName: "CFTYpeId");

   //         migrationBuilder.AlterColumn<string>(
   //             name: "CFTYpeId",
   //             table: "SplitTransactions",
   //             nullable: true,
   //             oldClrType: typeof(string),
   //             oldType: "nvarchar(450)",
   //             oldNullable: true);

			//migrationBuilder.AddColumn<string>(
			//	name: "CFTYpeID",
			//	table: "SplitTransactions",
			//	nullable: true);

			migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "SplitTransactions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AutomatedCashFlowsId",
                table: "AutomatedCashFlows",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Split",
                table: "AutomatedCashFlows",
                nullable: false,
                defaultValue: false);

            //migrationBuilder.CreateIndex(
            //    name: "IX_SplitTransactions_CFTYpeId",
            //    table: "SplitTransactions",
            //    column: "CFTYpeId");

            migrationBuilder.CreateIndex(
                name: "IX_AutomatedCashFlows_AutomatedCashFlowsId",
                table: "AutomatedCashFlows",
                column: "AutomatedCashFlowsId");

            migrationBuilder.AddForeignKey(
                name: "FK_AutomatedCashFlows_AutomatedCashFlows_AutomatedCashFlowsId",
                table: "AutomatedCashFlows",
                column: "AutomatedCashFlowsId",
                principalTable: "AutomatedCashFlows",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_SplitTransactions_CFTypes_CFTYpeId",
            //    table: "SplitTransactions",
            //    column: "CFTYpeId",
            //    principalTable: "CFTypes",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AutomatedCashFlows_AutomatedCashFlows_AutomatedCashFlowsId",
                table: "AutomatedCashFlows");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_SplitTransactions_CFTypes_CFTYpeID",
            //    table: "SplitTransactions");

            //migrationBuilder.DropIndex(
            //    name: "IX_SplitTransactions_CFTYpeID",
            //    table: "SplitTransactions");

            migrationBuilder.DropIndex(
                name: "IX_AutomatedCashFlows_AutomatedCashFlowsId",
                table: "AutomatedCashFlows");

            migrationBuilder.DropColumn(
                name: "CFTYpeID",
                table: "SplitTransactions");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "SplitTransactions");

            migrationBuilder.DropColumn(
                name: "AutomatedCashFlowsId",
                table: "AutomatedCashFlows");

            migrationBuilder.DropColumn(
                name: "Split",
                table: "AutomatedCashFlows");

            //migrationBuilder.RenameColumn(
            //    name: "CFTYpeId",
            //    table: "SplitTransactions",
            //    newName: "CFTYpeID");

            //migrationBuilder.AlterColumn<string>(
            //    name: "CFTYpeID",
            //    table: "SplitTransactions",
            //    type: "nvarchar(450)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldNullable: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_SplitTransactions_CFTYpeID",
            //    table: "SplitTransactions",
            //    column: "CFTYpeID");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_SplitTransactions_CFTypes_CFTYpeID",
            //    table: "SplitTransactions",
            //    column: "CFTYpeID",
            //    principalTable: "CFTypes",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);
        }
    }
}
