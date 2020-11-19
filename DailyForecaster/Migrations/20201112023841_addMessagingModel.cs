using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class addMessagingModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MessagingModel",
                columns: table => new
                {
                    MessagingModelId = table.Column<string>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    DateTimeCreated = table.Column<DateTime>(nullable: false),
                    DateTimeRead = table.Column<DateTime>(nullable: false),
                    RecipientId = table.Column<string>(nullable: true),
                    SenderId = table.Column<string>(nullable: true),
                    Delivered = table.Column<bool>(nullable: false),
                    Read = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessagingModel", x => x.MessagingModelId);
                    table.ForeignKey(
                        name: "FK_MessagingModel_FirebaseUser_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "FirebaseUser",
                        principalColumn: "FirebaseUserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MessagingModel_FirebaseUser_SenderId",
                        column: x => x.SenderId,
                        principalTable: "FirebaseUser",
                        principalColumn: "FirebaseUserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledTransactions",
                columns: table => new
                {
                    ScheduledTransactionsId = table.Column<string>(nullable: false),
                    Day = table.Column<int>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    CFTypeId = table.Column<string>(nullable: true),
                    FTypeId = table.Column<string>(nullable: true),
                    CFClassificationId = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    AccountId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledTransactions", x => x.ScheduledTransactionsId);
                    table.ForeignKey(
                        name: "FK_ScheduledTransactions_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScheduledTransactions_CFClassifications_CFClassificationId",
                        column: x => x.CFClassificationId,
                        principalTable: "CFClassifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScheduledTransactions_CFTypes_FTypeId",
                        column: x => x.FTypeId,
                        principalTable: "CFTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessagingModel_RecipientId",
                table: "MessagingModel",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_MessagingModel_SenderId",
                table: "MessagingModel",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledTransactions_AccountId",
                table: "ScheduledTransactions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledTransactions_CFClassificationId",
                table: "ScheduledTransactions",
                column: "CFClassificationId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledTransactions_FTypeId",
                table: "ScheduledTransactions",
                column: "FTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessagingModel");

            migrationBuilder.DropTable(
                name: "ScheduledTransactions");
        }
    }
}
