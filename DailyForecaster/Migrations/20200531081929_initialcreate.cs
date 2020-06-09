using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class initialcreate : Migration
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
                    TotalAmount = table.Column<double>(nullable: false),
                    DurationType = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    UserCreated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collections", x => x.CollectionsId);
                });

            migrationBuilder.CreateTable(
                name: "CollectionSharing",
                columns: table => new
                {
                    CollectionSharingId = table.Column<string>(nullable: false),
                    CollectionId = table.Column<string>(nullable: true),
                    count = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionSharing", x => x.CollectionSharingId);
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
                name: "BudgetTransactions",
                columns: table => new
                {
                    BudgetTransactionId = table.Column<string>(nullable: false),
                    BudgetId = table.Column<string>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    CFTypeId = table.Column<string>(nullable: false),
                    CFClassificationId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetTransactions", x => x.BudgetTransactionId);
                    table.ForeignKey(
                        name: "FK_BudgetTransactions_CFClassifications_CFClassificationId",
                        column: x => x.CFClassificationId,
                        principalTable: "CFClassifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetTransactions_CFTypes_CFTypeId",
                        column: x => x.CFTypeId,
                        principalTable: "CFTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "Budget",
                columns: table => new
                {
                    BudgetId = table.Column<string>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    CollectionId = table.Column<string>(nullable: false),
                    CollectionsObjCollectionsId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budget", x => x.BudgetId);
                    table.ForeignKey(
                        name: "FK_Budget_Collections_CollectionsObjCollectionsId",
                        column: x => x.CollectionsObjCollectionsId,
                        principalTable: "Collections",
                        principalColumn: "CollectionsId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserCollectionMapping",
                columns: table => new
                {
                    UserCollectionMappingId = table.Column<string>(nullable: false),
                    CollectionsId = table.Column<string>(nullable: false),
                    Id = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCollectionMapping", x => x.UserCollectionMappingId);
                    table.ForeignKey(
                        name: "FK_UserCollectionMapping_Collections_CollectionsId",
                        column: x => x.CollectionsId,
                        principalTable: "Collections",
                        principalColumn: "CollectionsId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCollectionMapping_AspNetUsers_Id",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
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
                name: "IX_Budget_CollectionsObjCollectionsId",
                table: "Budget",
                column: "CollectionsObjCollectionsId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetTransactions_CFClassificationId",
                table: "BudgetTransactions",
                column: "CFClassificationId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetTransactions_CFTypeId",
                table: "BudgetTransactions",
                column: "CFTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ManualCashFlows_CFClassificationId",
                table: "ManualCashFlows",
                column: "CFClassificationId");

            migrationBuilder.CreateIndex(
                name: "IX_ManualCashFlows_CFTypeId",
                table: "ManualCashFlows",
                column: "CFTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCollectionMapping_CollectionsId",
                table: "UserCollectionMapping",
                column: "CollectionsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCollectionMapping_Id",
                table: "UserCollectionMapping",
                column: "Id");
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
                name: "BudgetTransactions");

            migrationBuilder.DropTable(
                name: "CollectionSharing");

            migrationBuilder.DropTable(
                name: "UserCollectionMapping");

            migrationBuilder.DropTable(
                name: "Institution");

            migrationBuilder.DropTable(
                name: "ManualCashFlows");

            migrationBuilder.DropTable(
                name: "Collections");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "CFClassifications");

            migrationBuilder.DropTable(
                name: "CFTypes");
        }
    }
}
