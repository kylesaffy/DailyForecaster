using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
	public partial class mappingCorrection2 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_UserCollectionMapping_FirebaseUser_FirebaseUserID",
				table: "UserCollectionMapping");

			migrationBuilder.DropIndex(
				name: "IX_UserCollectionMapping_FirebaseUserID",
				table: "UserCollectionMapping");

			migrationBuilder.DropColumn(
				name: "FirebaseUserID",
				table: "UserCollectionMapping");

			migrationBuilder.AlterColumn<string>(
				name: "FirebaseUserId",
				table: "UserCollectionMapping",
				nullable: true,
				oldClrType: typeof(string),
				oldType: "nvarchar(max)",
				oldNullable: true);

			migrationBuilder.CreateIndex(
				name: "IX_UserCollectionMapping_FirebaseUserId",
				table: "UserCollectionMapping",
				column: "FirebaseUserId");

			migrationBuilder.AddForeignKey(
				name: "FK_UserCollectionMapping_FirebaseUser_FirebaseUserId",
				table: "UserCollectionMapping",
				column: "FirebaseUserId",
				principalTable: "FirebaseUser",
				principalColumn: "FirebaseUserId",
				onDelete: ReferentialAction.Restrict);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_UserCollectionMapping_FirebaseUser_FirebaseUserId",
				table: "UserCollectionMapping");

			migrationBuilder.DropIndex(
				name: "IX_UserCollectionMapping_FirebaseUserId",
				table: "UserCollectionMapping");

			migrationBuilder.AlterColumn<string>(
				name: "FirebaseUserId",
				table: "UserCollectionMapping",
				type: "nvarchar(max)",
				nullable: true,
				oldClrType: typeof(string),
				oldNullable: true);

			migrationBuilder.AddColumn<string>(
				name: "FirebaseUserID",
				table: "UserCollectionMapping",
				type: "nvarchar(450)",
				nullable: true);

			migrationBuilder.CreateIndex(
				name: "IX_UserCollectionMapping_FirebaseUserID",
				table: "UserCollectionMapping",
				column: "FirebaseUserID");

			migrationBuilder.AddForeignKey(
				name: "FK_UserCollectionMapping_FirebaseUser_FirebaseUserID",
				table: "UserCollectionMapping",
				column: "FirebaseUserID",
				principalTable: "FirebaseUser",
				principalColumn: "FirebaseUserId",
				onDelete: ReferentialAction.Restrict);
		}
	}
}
