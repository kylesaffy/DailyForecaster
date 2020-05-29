using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountCollectionsMapping",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CollectionId = table.Column<string>(nullable: true),
                    AccountId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountCollectionsMapping", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Budget",
                columns: table => new
                {
                    BudgetId = table.Column<string>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDFate = table.Column<DateTime>(nullable: false),
                    CollectionId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budget", x => x.BudgetId);
                });

            migrationBuilder.CreateTable(
                name: "CFClassifications",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Sign = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CFClassifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CFTypes",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Custom = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    ClientReference = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CFTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Collections",
                columns: table => new
                {
                    CollectionsId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    TotalAmount = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collections", x => x.CollectionsId);
                });

            migrationBuilder.CreateTable(
                name: "Institution",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    ContactNumber = table.Column<string>(nullable: false),
                    WebLink = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    BlobString = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Institution", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserCollectionMapping",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CollectionId = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCollectionMapping", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ManualCashFlows",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CFTypeId = table.Column<string>(nullable: false),
                    CFClassificationId = table.Column<string>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    DateBooked = table.Column<DateTime>(nullable: false),
                    DateCaptured = table.Column<DateTime>(nullable: false),
                    SourceOfExpense = table.Column<string>(nullable: false),
                    Expected = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ExpenseLocation = table.Column<string>(nullable: true),
                    PhotoBlobLink = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    AccountId = table.Column<string>(nullable: false),
                    isDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManualCashFlows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManualCashFlows_CFClassifications_CFClassificationId",
                        column: x => x.CFClassificationId,
                        principalTable: "CFClassifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ManualCashFlows_CFTypes_CFTypeId",
                        column: x => x.CFTypeId,
                        principalTable: "CFTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    InstitutionId = table.Column<string>(nullable: false),
                    Available = table.Column<double>(nullable: false),
                    AccountLimit = table.Column<double>(nullable: false),
                    NetAmount = table.Column<double>(nullable: false),
                    CollectionsId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Account_Collections_CollectionsId",
                        column: x => x.CollectionsId,
                        principalTable: "Collections",
                        principalColumn: "CollectionsId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Account_Institution_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "Institution",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AutomatedCashFlows",
                columns: table => new
                {
                    ID = table.Column<string>(nullable: false),
                    CFTypeId = table.Column<string>(nullable: false),
                    CFClassificationId = table.Column<string>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    DateBooked = table.Column<DateTime>(nullable: false),
                    DateCaptured = table.Column<DateTime>(nullable: false),
                    SourceOfExpense = table.Column<string>(nullable: false),
                    ManualCashFlowId = table.Column<string>(nullable: true),
                    AccountId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutomatedCashFlows", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AutomatedCashFlows_CFClassifications_CFClassificationId",
                        column: x => x.CFClassificationId,
                        principalTable: "CFClassifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AutomatedCashFlows_CFTypes_CFTypeId",
                        column: x => x.CFTypeId,
                        principalTable: "CFTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AutomatedCashFlows_ManualCashFlows_ManualCashFlowId",
                        column: x => x.ManualCashFlowId,
                        principalTable: "ManualCashFlows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_CollectionsId",
                table: "Account",
                column: "CollectionsId");

            migrationBuilder.CreateIndex(
                name: "IX_Account_InstitutionId",
                table: "Account",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_AutomatedCashFlows_CFClassificationId",
                table: "AutomatedCashFlows",
                column: "CFClassificationId");

            migrationBuilder.CreateIndex(
                name: "IX_AutomatedCashFlows_CFTypeId",
                table: "AutomatedCashFlows",
                column: "CFTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AutomatedCashFlows_ManualCashFlowId",
                table: "AutomatedCashFlows",
                column: "ManualCashFlowId");

            migrationBuilder.CreateIndex(
                name: "IX_ManualCashFlows_CFClassificationId",
                table: "ManualCashFlows",
                column: "CFClassificationId");

            migrationBuilder.CreateIndex(
                name: "IX_ManualCashFlows_CFTypeId",
                table: "ManualCashFlows",
                column: "CFTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "AccountCollectionsMapping");

            migrationBuilder.DropTable(
                name: "AutomatedCashFlows");

            migrationBuilder.DropTable(
                name: "Budget");

            migrationBuilder.DropTable(
                name: "UserCollectionMapping");

            migrationBuilder.DropTable(
                name: "Collections");

            migrationBuilder.DropTable(
                name: "Institution");

            migrationBuilder.DropTable(
                name: "ManualCashFlows");

            migrationBuilder.DropTable(
                name: "CFClassifications");

            migrationBuilder.DropTable(
                name: "CFTypes");
        }
    }
}
