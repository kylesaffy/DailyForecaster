using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DailyForecaster.Migrations
{
    public partial class expensemodels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductClassModel",
                columns: table => new
                {
                    ProductClassModelId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    SuperClass = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductClassModel", x => x.ProductClassModelId);
                });

            migrationBuilder.CreateTable(
                name: "RetailMerchants",
                columns: table => new
                {
                    RetailMerchantsId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RetailMerchants", x => x.RetailMerchantsId);
                });

            migrationBuilder.CreateTable(
                name: "ProductsModel",
                columns: table => new
                {
                    ProductsModelId = table.Column<string>(nullable: false),
                    ProductClassModelId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductsModel", x => x.ProductsModelId);
                    table.ForeignKey(
                        name: "FK_ProductsModel_ProductClassModel_ProductClassModelId",
                        column: x => x.ProductClassModelId,
                        principalTable: "ProductClassModel",
                        principalColumn: "ProductClassModelId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RetailBranches",
                columns: table => new
                {
                    RetailBranchesId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    ShopNumber = table.Column<string>(nullable: true),
                    Complex = table.Column<string>(nullable: true),
                    StreetNumber = table.Column<string>(nullable: true),
                    StreetName = table.Column<string>(nullable: true),
                    Suburb = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    PostalCode = table.Column<int>(nullable: false),
                    GoogleAddress = table.Column<string>(nullable: true),
                    RetailMerchantsId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RetailBranches", x => x.RetailBranchesId);
                    table.ForeignKey(
                        name: "FK_RetailBranches_RetailMerchants_RetailMerchantsId",
                        column: x => x.RetailMerchantsId,
                        principalTable: "RetailMerchants",
                        principalColumn: "RetailMerchantsId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemisedProducts",
                columns: table => new
                {
                    ItemisedProductsId = table.Column<string>(nullable: false),
                    ProductsId = table.Column<string>(nullable: true),
                    Products = table.Column<string>(nullable: true),
                    NumberOfProducts = table.Column<int>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    Savings = table.Column<double>(nullable: false),
                    SubTotal = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemisedProducts", x => x.ItemisedProductsId);
                    table.ForeignKey(
                        name: "FK_ItemisedProducts_ProductsModel_Products",
                        column: x => x.Products,
                        principalTable: "ProductsModel",
                        principalColumn: "ProductsModelId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExpenseModels",
                columns: table => new
                {
                    ExpenseModelId = table.Column<string>(nullable: false),
                    BlobLink = table.Column<string>(nullable: true),
                    ManualCashFlowId = table.Column<string>(nullable: true),
                    RetailBranchesId = table.Column<string>(nullable: true),
                    Total = table.Column<double>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseModels", x => x.ExpenseModelId);
                    table.ForeignKey(
                        name: "FK_ExpenseModels_ManualCashFlows_ManualCashFlowId",
                        column: x => x.ManualCashFlowId,
                        principalTable: "ManualCashFlows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExpenseModels_RetailBranches_RetailBranchesId",
                        column: x => x.RetailBranchesId,
                        principalTable: "RetailBranches",
                        principalColumn: "RetailBranchesId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseModels_ManualCashFlowId",
                table: "ExpenseModels",
                column: "ManualCashFlowId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseModels_RetailBranchesId",
                table: "ExpenseModels",
                column: "RetailBranchesId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemisedProducts_Products",
                table: "ItemisedProducts",
                column: "Products");

            migrationBuilder.CreateIndex(
                name: "IX_ProductsModel_ProductClassModelId",
                table: "ProductsModel",
                column: "ProductClassModelId");

            migrationBuilder.CreateIndex(
                name: "IX_RetailBranches_RetailMerchantsId",
                table: "RetailBranches",
                column: "RetailMerchantsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpenseModels");

            migrationBuilder.DropTable(
                name: "ItemisedProducts");

            migrationBuilder.DropTable(
                name: "RetailBranches");

            migrationBuilder.DropTable(
                name: "ProductsModel");

            migrationBuilder.DropTable(
                name: "RetailMerchants");

            migrationBuilder.DropTable(
                name: "ProductClassModel");
        }
    }
}
