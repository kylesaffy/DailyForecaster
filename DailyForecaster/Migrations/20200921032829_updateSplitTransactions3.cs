using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class updateSplitTransactions3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_SplitTransactions_CFTypes_CFTYpeId",
            //    table: "SplitTransactions");

            ////migrationBuilder.RenameColumn(
            ////    name: "CFTYpeId",
            ////    table: "SplitTransactions",
            ////    newName: "CFTYpeID");

            //migrationBuilder.RenameIndex(
            //    name: "IX_SplitTransactions_CFTYpeId",
            //    table: "SplitTransactions",
            //    newName: "IX_SplitTransactions_CFTYpeID");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_SplitTransactions_CFTypes_CFTYpeID",
            //    table: "SplitTransactions",
            //    column: "CFTYpeID",
            //    principalTable: "CFTypes",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_SplitTransactions_CFTypes_CFTYpeID",
            //    table: "SplitTransactions");

            ////migrationBuilder.RenameColumn(
            ////    name: "CFTYpeID",
            ////    table: "SplitTransactions",
            ////    newName: "CFTYpeId");

            //migrationBuilder.RenameIndex(
            //    name: "IX_SplitTransactions_CFTYpeID",
            //    table: "SplitTransactions",
            //    newName: "IX_SplitTransactions_CFTYpeId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_SplitTransactions_CFTypes_CFTYpeId",
            //    table: "SplitTransactions",
            //    column: "CFTYpeId",
            //    principalTable: "CFTypes",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);
        }
    }
}
