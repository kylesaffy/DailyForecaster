using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class updateSplitTransactions2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
   //         migrationBuilder.DropForeignKey(
			//			name: "FK_SplitTransactions_CFTypes_CFTYpeID",
			//			table: "SplitTransactions");

			//migrationBuilder.DropIndex(
			//	name: "IX_SplitTransactions_CFTYpeID",
			//	table: "SplitTransactions");

			//migrationBuilder.RenameColumn(
			//	name: "CFTYpeID",
			//	table: "SplitTransactions",
			//	newName: "CFTYpeId");

			//migrationBuilder.AlterColumn<string>(
			//	name: "CFTYpeId",
			//	table: "SplitTransactions",
			//	nullable: true,
			//	oldClrType: typeof(string),
			//	oldType: "nvarchar(450)",
			//	oldNullable: true);

			//migrationBuilder.AddColumn<string>(
			//	name: "CFTYpeID",
			//	table: "SplitTransactions",
			//	nullable: true);
			//migrationBuilder.DropForeignKey(
			//	 name: "FK_SplitTransactions_CFTypes_CFTYpeID",
   //             table: "SplitTransactions");

   //         migrationBuilder.DropIndex(
   //             name: "IX_SplitTransactions_CFTYpeID",
   //             table: "SplitTransactions");

   //         migrationBuilder.DropColumn(
   //             name: "CFTYpeID",
   //             table: "SplitTransactions");

            //migrationBuilder.AlterColumn<string>(
            //    name: "CFTYpeId",
            //    table: "SplitTransactions",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_SplitTransactions_CFTYpeId",
            //    table: "SplitTransactions",
            //    column: "CFTYpeId");

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
            //migrationBuilder.DropForeignKey(
            //    name: "FK_SplitTransactions_CFTypes_CFTYpeId",
            //    table: "SplitTransactions");

            //migrationBuilder.DropIndex(
            //    name: "IX_SplitTransactions_CFTYpeId",
            //    table: "SplitTransactions");

            //migrationBuilder.AlterColumn<string>(
            //    name: "CFTYpeId",
            //    table: "SplitTransactions",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldNullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "CFTYpeID",
            //    table: "SplitTransactions",
            //    type: "nvarchar(450)",
            //    nullable: true);

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
