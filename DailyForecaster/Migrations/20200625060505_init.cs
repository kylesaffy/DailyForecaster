using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class init : Migration
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
                name: "AccountType",
                columns: table => new
                {
                    AccountTypeId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Transactional = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountType", x => x.AccountTypeId);
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
                name: "ClickTracker",
                columns: table => new
                {
                    ClickTrackerId = table.Column<string>(nullable: false),
                    Location = table.Column<string>(nullable: true),
                    GET = table.Column<bool>(nullable: false),
                    POST = table.Column<bool>(nullable: false),
                    RecordDateTime = table.Column<DateTime>(nullable: false),
                    RequestData = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClickTracker", x => x.ClickTrackerId);
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
                name: "RateInformation",
                columns: table => new
                {
                    RateInformationId = table.Column<string>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateEffective = table.Column<DateTime>(nullable: false),
                    RepoRate = table.Column<double>(nullable: false),
                    PrimeRate = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RateInformation", x => x.RateInformationId);
                });

            migrationBuilder.CreateTable(
                name: "SimulationAssumptions",
                columns: table => new
                {
                    SimulationAssumptionsId = table.Column<string>(nullable: false),
                    NumberOfMonths = table.Column<int>(nullable: false),
                    Bonus = table.Column<bool>(nullable: false),
                    BonusMonth = table.Column<int>(nullable: false),
                    BonusAmount = table.Column<double>(nullable: false),
                    Increase = table.Column<bool>(nullable: false),
                    IncreaseMonth = table.Column<int>(nullable: false),
                    IncreasePercentage = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimulationAssumptions", x => x.SimulationAssumptionsId);
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
                    UserCreated = table.Column<string>(nullable: true),
                    ResetDay = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collections", x => x.CollectionsId);
                    table.ForeignKey(
                        name: "FK_Collections_AspNetUsers_UserCreated",
                        column: x => x.UserCreated,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    DebitRate = table.Column<double>(nullable: false),
                    CreditRate = table.Column<double>(nullable: false),
                    Floating = table.Column<bool>(nullable: false),
                    FloatingType = table.Column<string>(nullable: true),
                    MonthlyFee = table.Column<double>(nullable: false),
                    CollectionsId = table.Column<string>(nullable: true),
                    AccountTypeId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Account_AccountType_AccountTypeId",
                        column: x => x.AccountTypeId,
                        principalTable: "AccountType",
                        principalColumn: "AccountTypeId",
                        onDelete: ReferentialAction.Restrict);
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
                name: "Simualtion",
                columns: table => new
                {
                    SimulationId = table.Column<string>(nullable: false),
                    SimulationName = table.Column<string>(nullable: false),
                    CollectionsId = table.Column<string>(nullable: false),
                    SimulationAssumptionsId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Simualtion", x => x.SimulationId);
                    table.ForeignKey(
                        name: "FK_Simualtion_Collections_CollectionsId",
                        column: x => x.CollectionsId,
                        principalTable: "Collections",
                        principalColumn: "CollectionsId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Simualtion_SimulationAssumptions_SimulationAssumptionsId",
                        column: x => x.SimulationAssumptionsId,
                        principalTable: "SimulationAssumptions",
                        principalColumn: "SimulationAssumptionsId",
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
                        name: "FK_ManualCashFlows_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    SimulationId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budget", x => x.BudgetId);
                    table.ForeignKey(
                        name: "FK_Budget_Collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collections",
                        principalColumn: "CollectionsId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Budget_Simualtion_SimulationId",
                        column: x => x.SimulationId,
                        principalTable: "Simualtion",
                        principalColumn: "SimulationId",
                        onDelete: ReferentialAction.Restrict);
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

            migrationBuilder.CreateTable(
                name: "BudgetTransactions",
                columns: table => new
                {
                    BudgetTransactionId = table.Column<string>(nullable: false),
                    BudgetId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Amount = table.Column<double>(nullable: false),
                    CFTypeId = table.Column<string>(nullable: false),
                    CFClassificationId = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetTransactions", x => x.BudgetTransactionId);
                    table.ForeignKey(
                        name: "FK_BudgetTransactions_Budget_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budget",
                        principalColumn: "BudgetId",
                        onDelete: ReferentialAction.Cascade);
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
                    table.ForeignKey(
                        name: "FK_BudgetTransactions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountChange",
                columns: table => new
                {
                    AccountChangeId = table.Column<string>(nullable: false),
                    UpdatedBalance = table.Column<double>(nullable: false),
                    DateAdded = table.Column<DateTime>(nullable: false),
                    ManualCashFlowId = table.Column<string>(nullable: true),
                    AutomatedCashFlowId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountChange", x => x.AccountChangeId);
                    table.ForeignKey(
                        name: "FK_AccountChange_AutomatedCashFlows_AutomatedCashFlowId",
                        column: x => x.AutomatedCashFlowId,
                        principalTable: "AutomatedCashFlows",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountChange_ManualCashFlows_ManualCashFlowId",
                        column: x => x.ManualCashFlowId,
                        principalTable: "ManualCashFlows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    NotesId = table.Column<string>(nullable: false),
                    BudgetTransactionId = table.Column<string>(nullable: false),
                    Body = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notes", x => x.NotesId);
                    table.ForeignKey(
                        name: "FK_Notes_BudgetTransactions_BudgetTransactionId",
                        column: x => x.BudgetTransactionId,
                        principalTable: "BudgetTransactions",
                        principalColumn: "BudgetTransactionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_AccountTypeId",
                table: "Account",
                column: "AccountTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Account_CollectionsId",
                table: "Account",
                column: "CollectionsId");

            migrationBuilder.CreateIndex(
                name: "IX_Account_InstitutionId",
                table: "Account",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountChange_AutomatedCashFlowId",
                table: "AccountChange",
                column: "AutomatedCashFlowId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountChange_ManualCashFlowId",
                table: "AccountChange",
                column: "ManualCashFlowId");

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
                name: "IX_Budget_CollectionId",
                table: "Budget",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Budget_SimulationId",
                table: "Budget",
                column: "SimulationId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetTransactions_BudgetId",
                table: "BudgetTransactions",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetTransactions_CFClassificationId",
                table: "BudgetTransactions",
                column: "CFClassificationId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetTransactions_CFTypeId",
                table: "BudgetTransactions",
                column: "CFTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetTransactions_UserId",
                table: "BudgetTransactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Collections_UserCreated",
                table: "Collections",
                column: "UserCreated");

            migrationBuilder.CreateIndex(
                name: "IX_ManualCashFlows_AccountId",
                table: "ManualCashFlows",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ManualCashFlows_CFClassificationId",
                table: "ManualCashFlows",
                column: "CFClassificationId");

            migrationBuilder.CreateIndex(
                name: "IX_ManualCashFlows_CFTypeId",
                table: "ManualCashFlows",
                column: "CFTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_BudgetTransactionId",
                table: "Notes",
                column: "BudgetTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Simualtion_CollectionsId",
                table: "Simualtion",
                column: "CollectionsId");

            migrationBuilder.CreateIndex(
                name: "IX_Simualtion_SimulationAssumptionsId",
                table: "Simualtion",
                column: "SimulationAssumptionsId",
                unique: true,
                filter: "[SimulationAssumptionsId] IS NOT NULL");

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
                name: "AccountChange");

            migrationBuilder.DropTable(
                name: "AccountCollectionsMapping");

            migrationBuilder.DropTable(
                name: "ClickTracker");

            migrationBuilder.DropTable(
                name: "CollectionSharing");

            migrationBuilder.DropTable(
                name: "Notes");

            migrationBuilder.DropTable(
                name: "RateInformation");

            migrationBuilder.DropTable(
                name: "UserCollectionMapping");

            migrationBuilder.DropTable(
                name: "AutomatedCashFlows");

            migrationBuilder.DropTable(
                name: "BudgetTransactions");

            migrationBuilder.DropTable(
                name: "ManualCashFlows");

            migrationBuilder.DropTable(
                name: "Budget");

            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "CFClassifications");

            migrationBuilder.DropTable(
                name: "CFTypes");

            migrationBuilder.DropTable(
                name: "Simualtion");

            migrationBuilder.DropTable(
                name: "AccountType");

            migrationBuilder.DropTable(
                name: "Institution");

            migrationBuilder.DropTable(
                name: "Collections");

            migrationBuilder.DropTable(
                name: "SimulationAssumptions");

        }
    }
}
