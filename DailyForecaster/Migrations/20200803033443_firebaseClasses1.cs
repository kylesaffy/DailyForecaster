using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class firebaseClasses1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FirebaseLogin",
                table: "FirebaseLogin");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "FirebaseLogin");

            migrationBuilder.DropColumn(
                name: "DateTime",
                table: "FirebaseLogin");

            migrationBuilder.AddColumn<string>(
                name: "FirebaseLoginId",
                table: "FirebaseLogin",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LoginDate",
                table: "FirebaseLogin",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_FirebaseLogin",
                table: "FirebaseLogin",
                column: "FirebaseLoginId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FirebaseLogin",
                table: "FirebaseLogin");

            migrationBuilder.DropColumn(
                name: "FirebaseLoginId",
                table: "FirebaseLogin");

            migrationBuilder.DropColumn(
                name: "LoginDate",
                table: "FirebaseLogin");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "FirebaseLogin",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTime",
                table: "FirebaseLogin",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_FirebaseLogin",
                table: "FirebaseLogin",
                column: "Id");
        }
    }
}
