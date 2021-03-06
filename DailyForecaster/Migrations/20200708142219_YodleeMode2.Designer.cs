﻿// <auto-generated />
using System;
using DailyForecaster.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DailyForecaster.Migrations
{
    [DbContext(typeof(FinPlannerContext))]
    [Migration("20200708142219_YodleeMode2")]
    partial class YodleeMode2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DailyForecaster.Models.Account", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<double>("AccountLimit")
                        .HasColumnType("float");

                    b.Property<string>("AccountTypeId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<double>("Available")
                        .HasColumnType("float");

                    b.Property<string>("CollectionsId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<double>("CreditRate")
                        .HasColumnType("float");

                    b.Property<double>("DebitRate")
                        .HasColumnType("float");

                    b.Property<bool>("Floating")
                        .HasColumnType("bit");

                    b.Property<string>("FloatingType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InstitutionId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<double>("MonthlyFee")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("NetAmount")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("AccountTypeId");

                    b.HasIndex("CollectionsId");

                    b.HasIndex("InstitutionId");

                    b.ToTable("Account");
                });

            modelBuilder.Entity("DailyForecaster.Models.AccountChange", b =>
                {
                    b.Property<string>("AccountChangeId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AutomatedCashFlowId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("datetime2");

                    b.Property<string>("ManualCashFlowId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<double>("UpdatedBalance")
                        .HasColumnType("float");

                    b.HasKey("AccountChangeId");

                    b.HasIndex("AutomatedCashFlowId");

                    b.HasIndex("ManualCashFlowId");

                    b.ToTable("AccountChange");
                });

            modelBuilder.Entity("DailyForecaster.Models.AccountCollectionsMapping", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AccountId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CollectionId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AccountCollectionsMapping");
                });

            modelBuilder.Entity("DailyForecaster.Models.AccountType", b =>
                {
                    b.Property<string>("AccountTypeId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Transactional")
                        .HasColumnType("bit");

                    b.HasKey("AccountTypeId");

                    b.ToTable("AccountType");
                });

            modelBuilder.Entity("DailyForecaster.Models.AspNetUsers", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128);

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("DailyForecaster.Models.AutomatedCashFlow", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AccountId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Amount")
                        .HasColumnType("float");

                    b.Property<string>("CFClassificationId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CFTypeId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("DateBooked")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateCaptured")
                        .HasColumnType("datetime2");

                    b.Property<string>("ManualCashFlowId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("SourceOfExpense")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("CFClassificationId");

                    b.HasIndex("CFTypeId");

                    b.HasIndex("ManualCashFlowId");

                    b.ToTable("AutomatedCashFlows");
                });

            modelBuilder.Entity("DailyForecaster.Models.Budget", b =>
                {
                    b.Property<string>("BudgetId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CollectionId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("SimulationId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.HasKey("BudgetId");

                    b.HasIndex("CollectionId");

                    b.HasIndex("SimulationId");

                    b.ToTable("Budget");
                });

            modelBuilder.Entity("DailyForecaster.Models.BudgetTransaction", b =>
                {
                    b.Property<string>("BudgetTransactionId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<double>("Amount")
                        .HasColumnType("float");

                    b.Property<string>("BudgetId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CFClassificationId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CFTypeId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(128)");

                    b.HasKey("BudgetTransactionId");

                    b.HasIndex("BudgetId");

                    b.HasIndex("CFClassificationId");

                    b.HasIndex("CFTypeId");

                    b.HasIndex("UserId");

                    b.ToTable("BudgetTransactions");
                });

            modelBuilder.Entity("DailyForecaster.Models.CFClassification", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Sign")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("CFClassifications");
                });

            modelBuilder.Entity("DailyForecaster.Models.CFType", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ClientReference")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Custom")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("CFTypes");
                });

            modelBuilder.Entity("DailyForecaster.Models.ClickTracker", b =>
                {
                    b.Property<string>("ClickTrackerId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("GET")
                        .HasColumnType("bit");

                    b.Property<string>("Location")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("POST")
                        .HasColumnType("bit");

                    b.Property<DateTime>("RecordDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("RequestData")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ClickTrackerId");

                    b.ToTable("ClickTracker");
                });

            modelBuilder.Entity("DailyForecaster.Models.CollectionSharing", b =>
                {
                    b.Property<string>("CollectionSharingId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CollectionId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("count")
                        .HasColumnType("int");

                    b.HasKey("CollectionSharingId");

                    b.ToTable("CollectionSharing");
                });

            modelBuilder.Entity("DailyForecaster.Models.Collections", b =>
                {
                    b.Property<string>("CollectionsId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<string>("DurationType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ResetDay")
                        .HasColumnType("int");

                    b.Property<double>("TotalAmount")
                        .HasColumnType("float");

                    b.Property<string>("UserCreated")
                        .HasColumnType("nvarchar(128)");

                    b.HasKey("CollectionsId");

                    b.HasIndex("UserCreated");

                    b.ToTable("Collections");
                });

            modelBuilder.Entity("DailyForecaster.Models.EmailStore", b =>
                {
                    b.Property<string>("EmailStoreId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Body")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EmailDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("From")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Subject")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("To")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("EmailStoreId");

                    b.ToTable("EmailStore");
                });

            modelBuilder.Entity("DailyForecaster.Models.ExceptionCatcher", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Exception")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ExceptionCatcher");
                });

            modelBuilder.Entity("DailyForecaster.Models.Institution", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("BlobString")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ContactNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("WebLink")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Institution");
                });

            modelBuilder.Entity("DailyForecaster.Models.ManualCashFlow", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AccountId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<double>("Amount")
                        .HasColumnType("float");

                    b.Property<string>("CFClassificationId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CFTypeId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("DateBooked")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateCaptured")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Expected")
                        .HasColumnType("bit");

                    b.Property<string>("ExpenseLocation")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhotoBlobLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SourceOfExpense")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isDeleted")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("CFClassificationId");

                    b.HasIndex("CFTypeId");

                    b.ToTable("ManualCashFlows");
                });

            modelBuilder.Entity("DailyForecaster.Models.Notes", b =>
                {
                    b.Property<string>("NotesId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BudgetTransactionId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("NotesId");

                    b.HasIndex("BudgetTransactionId");

                    b.ToTable("Notes");
                });

            modelBuilder.Entity("DailyForecaster.Models.RateInformation", b =>
                {
                    b.Property<string>("RateInformationId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateEffective")
                        .HasColumnType("datetime2");

                    b.Property<double>("PrimeRate")
                        .HasColumnType("float");

                    b.Property<double>("RepoRate")
                        .HasColumnType("float");

                    b.HasKey("RateInformationId");

                    b.ToTable("RateInformation");
                });

            modelBuilder.Entity("DailyForecaster.Models.ReportedTransaction", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AccountId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<double>("Amount")
                        .HasColumnType("float");

                    b.Property<string>("CFClassificationId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CFTypeId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("DateCaptured")
                        .HasColumnType("datetime2");

                    b.Property<string>("ManualCashFlowId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("SourceOfExpense")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("CFClassificationId");

                    b.HasIndex("CFTypeId");

                    b.HasIndex("ManualCashFlowId");

                    b.ToTable("ReportedTransaction");
                });

            modelBuilder.Entity("DailyForecaster.Models.Simulation", b =>
                {
                    b.Property<string>("SimulationId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CollectionsId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("SimulationAssumptionsId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("SimulationName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SimulationId");

                    b.HasIndex("CollectionsId");

                    b.HasIndex("SimulationAssumptionsId")
                        .IsUnique()
                        .HasFilter("[SimulationAssumptionsId] IS NOT NULL");

                    b.ToTable("Simualtion");
                });

            modelBuilder.Entity("DailyForecaster.Models.SimulationAssumptions", b =>
                {
                    b.Property<string>("SimulationAssumptionsId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("Bonus")
                        .HasColumnType("bit");

                    b.Property<double>("BonusAmount")
                        .HasColumnType("float");

                    b.Property<int>("BonusMonth")
                        .HasColumnType("int");

                    b.Property<bool>("Increase")
                        .HasColumnType("bit");

                    b.Property<int>("IncreaseMonth")
                        .HasColumnType("int");

                    b.Property<double>("IncreasePercentage")
                        .HasColumnType("float");

                    b.Property<int>("NumberOfMonths")
                        .HasColumnType("int");

                    b.HasKey("SimulationAssumptionsId");

                    b.ToTable("SimulationAssumptions");
                });

            modelBuilder.Entity("DailyForecaster.Models.UserCollectionMapping", b =>
                {
                    b.Property<string>("UserCollectionMappingId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CollectionsId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Id")
                        .IsRequired()
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128);

                    b.HasKey("UserCollectionMappingId");

                    b.HasIndex("CollectionsId");

                    b.HasIndex("Id");

                    b.ToTable("UserCollectionMapping");
                });

            modelBuilder.Entity("DailyForecaster.Models.YodleeModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CollectionsId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("loginName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CollectionsId");

                    b.ToTable("YodleeModel");
                });

            modelBuilder.Entity("DailyForecaster.Models.Account", b =>
                {
                    b.HasOne("DailyForecaster.Models.AccountType", "AccountType")
                        .WithMany("Accounts")
                        .HasForeignKey("AccountTypeId");

                    b.HasOne("DailyForecaster.Models.Collections", "Collections")
                        .WithMany("Accounts")
                        .HasForeignKey("CollectionsId");

                    b.HasOne("DailyForecaster.Models.Institution", "Institution")
                        .WithMany()
                        .HasForeignKey("InstitutionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DailyForecaster.Models.AccountChange", b =>
                {
                    b.HasOne("DailyForecaster.Models.AutomatedCashFlow", "AutomatedCashFlow")
                        .WithMany()
                        .HasForeignKey("AutomatedCashFlowId");

                    b.HasOne("DailyForecaster.Models.ManualCashFlow", "ManualCashFlow")
                        .WithMany()
                        .HasForeignKey("ManualCashFlowId");
                });

            modelBuilder.Entity("DailyForecaster.Models.AutomatedCashFlow", b =>
                {
                    b.HasOne("DailyForecaster.Models.CFClassification", "CFClassification")
                        .WithMany()
                        .HasForeignKey("CFClassificationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DailyForecaster.Models.CFType", "CFType")
                        .WithMany()
                        .HasForeignKey("CFTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DailyForecaster.Models.ManualCashFlow", "ManualCashFlow")
                        .WithMany()
                        .HasForeignKey("ManualCashFlowId");
                });

            modelBuilder.Entity("DailyForecaster.Models.Budget", b =>
                {
                    b.HasOne("DailyForecaster.Models.Collections", "Collection")
                        .WithMany("Budgets")
                        .HasForeignKey("CollectionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DailyForecaster.Models.Simulation", null)
                        .WithMany("Budgets")
                        .HasForeignKey("SimulationId");
                });

            modelBuilder.Entity("DailyForecaster.Models.BudgetTransaction", b =>
                {
                    b.HasOne("DailyForecaster.Models.Budget", "Budget")
                        .WithMany("BudgetTransactions")
                        .HasForeignKey("BudgetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DailyForecaster.Models.CFClassification", "CFClassification")
                        .WithMany("BudgetTransactions")
                        .HasForeignKey("CFClassificationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DailyForecaster.Models.CFType", "CFType")
                        .WithMany("BudgetTransactions")
                        .HasForeignKey("CFTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DailyForecaster.Models.AspNetUsers", "AspNetUsers")
                        .WithMany("BudgetTransactions")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("DailyForecaster.Models.Collections", b =>
                {
                    b.HasOne("DailyForecaster.Models.AspNetUsers", "AspNetUsers")
                        .WithMany("Collections")
                        .HasForeignKey("UserCreated");
                });

            modelBuilder.Entity("DailyForecaster.Models.ManualCashFlow", b =>
                {
                    b.HasOne("DailyForecaster.Models.Account", "Account")
                        .WithMany("ManualCashFlows")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DailyForecaster.Models.CFClassification", "CFClassification")
                        .WithMany("ManualCashFlows")
                        .HasForeignKey("CFClassificationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DailyForecaster.Models.CFType", "CFType")
                        .WithMany()
                        .HasForeignKey("CFTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DailyForecaster.Models.Notes", b =>
                {
                    b.HasOne("DailyForecaster.Models.BudgetTransaction", "BudgetTransaction")
                        .WithMany("Notes")
                        .HasForeignKey("BudgetTransactionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DailyForecaster.Models.ReportedTransaction", b =>
                {
                    b.HasOne("DailyForecaster.Models.Account", null)
                        .WithMany("ReportedTransactions")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DailyForecaster.Models.CFClassification", "CFClassification")
                        .WithMany()
                        .HasForeignKey("CFClassificationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DailyForecaster.Models.CFType", "CFType")
                        .WithMany()
                        .HasForeignKey("CFTypeId");

                    b.HasOne("DailyForecaster.Models.ManualCashFlow", "ManualCashFlow")
                        .WithMany()
                        .HasForeignKey("ManualCashFlowId");
                });

            modelBuilder.Entity("DailyForecaster.Models.Simulation", b =>
                {
                    b.HasOne("DailyForecaster.Models.Collections", "Collections")
                        .WithMany("Simualtions")
                        .HasForeignKey("CollectionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DailyForecaster.Models.SimulationAssumptions", "SimulationAssumptions")
                        .WithOne("Simulation")
                        .HasForeignKey("DailyForecaster.Models.Simulation", "SimulationAssumptionsId");
                });

            modelBuilder.Entity("DailyForecaster.Models.UserCollectionMapping", b =>
                {
                    b.HasOne("DailyForecaster.Models.Collections", "Collections")
                        .WithMany("UserCollectionMappings")
                        .HasForeignKey("CollectionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DailyForecaster.Models.AspNetUsers", "AspNetUsers")
                        .WithMany("UserCollectionMappings")
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DailyForecaster.Models.YodleeModel", b =>
                {
                    b.HasOne("DailyForecaster.Models.Collections", "Collection")
                        .WithMany()
                        .HasForeignKey("CollectionsId");
                });
#pragma warning restore 612, 618
        }
    }
}
