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
    [Migration("20200531082933_AddingFKtoCollections")]
    partial class AddingFKtoCollections
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DailyForecaster.Models.Account", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<double>("AccountLimit")
                        .HasColumnType("float");

                    b.Property<double>("Available")
                        .HasColumnType("float");

                    b.Property<string>("CollectionsId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("InstitutionId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("NetAmount")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("CollectionsId");

                    b.HasIndex("InstitutionId");

                    b.ToTable("Account");
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
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CollectionsObjCollectionsId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.HasKey("BudgetId");

                    b.HasIndex("CollectionsObjCollectionsId");

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
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CFClassificationId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CFTypeId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("BudgetTransactionId");

                    b.HasIndex("CFClassificationId");

                    b.HasIndex("CFTypeId");

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

                    b.Property<double>("TotalAmount")
                        .HasColumnType("float");

                    b.Property<string>("UserCreated")
                        .HasColumnType("nvarchar(128)");

                    b.HasKey("CollectionsId");

                    b.HasIndex("UserCreated");

                    b.ToTable("Collections");
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

                    b.HasIndex("CFClassificationId");

                    b.HasIndex("CFTypeId");

                    b.ToTable("ManualCashFlows");
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

            modelBuilder.Entity("DailyForecaster.Models.Account", b =>
                {
                    b.HasOne("DailyForecaster.Models.Collections", null)
                        .WithMany("Accounts")
                        .HasForeignKey("CollectionsId");

                    b.HasOne("DailyForecaster.Models.Institution", "Institution")
                        .WithMany()
                        .HasForeignKey("InstitutionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
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
                    b.HasOne("DailyForecaster.Models.Collections", "CollectionsObj")
                        .WithMany()
                        .HasForeignKey("CollectionsObjCollectionsId");
                });

            modelBuilder.Entity("DailyForecaster.Models.BudgetTransaction", b =>
                {
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
                });

            modelBuilder.Entity("DailyForecaster.Models.Collections", b =>
                {
                    b.HasOne("DailyForecaster.Models.AspNetUsers", "AspNetUsers")
                        .WithMany("Collections")
                        .HasForeignKey("UserCreated");
                });

            modelBuilder.Entity("DailyForecaster.Models.ManualCashFlow", b =>
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
#pragma warning restore 612, 618
        }
    }
}
