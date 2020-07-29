using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class AmortAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Amortised",
                table: "AccountType",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "Maturity",
                table: "Account",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<double>(
                name: "MonthlyPayment",
                table: "Account",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "AccountAmortisation",
                columns: table => new
                {
                    AccountAmortisationId = table.Column<string>(nullable: false),
                    AccountId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountAmortisation", x => x.AccountAmortisationId);
                    table.ForeignKey(
                        name: "FK_AccountAmortisation_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyAmortisation",
                columns: table => new
                {
                    MonthlyAmortisationId = table.Column<string>(nullable: false),
                    AccountAmortisationId = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    Open = table.Column<double>(nullable: false),
                    Interest = table.Column<double>(nullable: false),
                    Payment = table.Column<double>(nullable: false),
                    Capital = table.Column<double>(nullable: false),
                    Additional = table.Column<double>(nullable: false),
                    Close = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyAmortisation", x => x.MonthlyAmortisationId);
                    table.ForeignKey(
                        name: "FK_MonthlyAmortisation_AccountAmortisation_AccountAmortisationId",
                        column: x => x.AccountAmortisationId,
                        principalTable: "AccountAmortisation",
                        principalColumn: "AccountAmortisationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentModel",
                columns: table => new
                {
                    PaymentModelId = table.Column<string>(nullable: false),
                    AccountAmortisationId = table.Column<string>(nullable: true),
                    LoanInstallment = table.Column<double>(nullable: false),
                    NonLoanPortion = table.Column<double>(nullable: false),
                    CostOfLoan = table.Column<double>(nullable: false),
                    AdditionalLoan = table.Column<double>(nullable: false),
                    TotalPayable = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentModel", x => x.PaymentModelId);
                    table.ForeignKey(
                        name: "FK_PaymentModel_AccountAmortisation_AccountAmortisationId",
                        column: x => x.AccountAmortisationId,
                        principalTable: "AccountAmortisation",
                        principalColumn: "AccountAmortisationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountAmortisation_AccountId",
                table: "AccountAmortisation",
                column: "AccountId",
                unique: true,
                filter: "[AccountId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyAmortisation_AccountAmortisationId",
                table: "MonthlyAmortisation",
                column: "AccountAmortisationId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentModel_AccountAmortisationId",
                table: "PaymentModel",
                column: "AccountAmortisationId",
                unique: true,
                filter: "[AccountAmortisationId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonthlyAmortisation");

            migrationBuilder.DropTable(
                name: "PaymentModel");

            migrationBuilder.DropTable(
                name: "AccountAmortisation");

            migrationBuilder.DropColumn(
                name: "Amortised",
                table: "AccountType");

            migrationBuilder.DropColumn(
                name: "Maturity",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "MonthlyPayment",
                table: "Account");
        }
    }
}
