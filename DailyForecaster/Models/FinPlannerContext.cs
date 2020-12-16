using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;

namespace DailyForecaster.Models
{
    public partial class FinPlannerContext : DbContext
    {
        public FinPlannerContext(DbContextOptions<FinPlannerContext> options) : base(options) { }
        public FinPlannerContext() { }
        public DbSet<CFType> CFTypes { get; set; }
        public DbSet<CFClassification> CFClassifications { get; set; }

        //public DbSet<CashFlowItem> CashFlowItems { get; set; }
        public DbSet<ManualCashFlow> ManualCashFlows { get; set; }
        public DbSet<AutomatedCashFlow> AutomatedCashFlows { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<Collections> Collections { get; set; }
        public DbSet<UserCollectionMapping> UserCollectionMapping { get; set; }
        public DbSet<Institution> Institution { get; set; }
        public DbSet<AccountCollectionsMapping> AccountCollectionsMapping { get; set; }
        public DbSet<Budget> Budget { get; set; }
        public DbSet<BudgetTransaction> BudgetTransactions { get; set; }
        public DbSet<CollectionSharing> CollectionSharing { get; set; }
        public DbSet<AspNetUsers> AspNetUsers { get; set; }
        public DbSet<RateInformation> RateInformation { get; set; }
        public DbSet<AccountType> AccountType { get; set; }
        public DbSet<Simulation> Simulation { get; set; }
        public DbSet<SimulationAssumptions> SimulationAssumptions { get; set; }
        public DbSet<Notes> Notes { get; set; }
        public DbSet<AccountChange> AccountChange { get; set; }
        public DbSet<ClickTracker> ClickTracker { get; set; }
        public DbSet<ExceptionCatcher> ExceptionCatcher { get; set; }
        public DbSet<YodleeModel> YodleeModel { get; set; }
        public DbSet<EmailStore> EmailStore { get; set; }
        public DbSet<AutomatedLog> AutomatedLog { get; set; }
        public DbSet<MonthlyAmortisation> MonthlyAmortisation { get; set; }
        public DbSet<PaymentModel> PaymentModel { get; set; }
        public DbSet<AccountAmortisation> AccountAmortisation { get; set; }
        public DbSet<AccountBalance> AccountBalance { get; set; }
        public DbSet<FirebaseUser> FirebaseUser { get; set; }
        public DbSet<FirebaseLogin> FirebaseLogin { get; set; }
        public DbSet<AccountState> AccountState { get; set; }
        public DbSet<AppAreas> AppAreas { get; set; }
        public DbSet<UserInteraction> UserInteraction { get; set; }
        public DbSet<EmailPreferences> EmailPrefernces { get; set; }
        public DbSet<EmailRecords> EmailRecords { get; set; }
        public DbSet<LogoffModel> LogoffModel { get; set; }
        public DbSet<SplitTransactions> SplitTransactions { get; set; }
        public DbSet<ProductClassModel> ProductClassModel { get; set; }
        public DbSet<ProductsModel> ProductsModel { get; set; }
        public DbSet<ItemisedProducts> ItemisedProducts { get; set; }
        public DbSet<RetailMerchants> RetailMerchants { get; set; }
        public DbSet<RetailBranches> RetailBranches { get; set; }
        public DbSet<ExpenseModel> ExpenseModels { get; set; }
        public DbSet<DailyTip> DailyTip { get; set; }
        public DbSet<DailyMotivational> DailyMotivational {get;set;}
        public DbSet<IncludeYodlee> IncludeYodlee { get; set; }
        public DbSet<ScheduledTransactions> ScheduledTransactions { get; set; }
        public DbSet<MessagingModel> MessagingModel { get; set; }
        public DbSet<YodleeAccountType> YodleeAccountType { get; set; }
        public DbSet<BonusModel> BonusModel { get; set; }
        public DbSet<IncreaseModel> IncreaseModel { get; set; }
        public DbSet<PaymentLink> PaymentLink { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .Build();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }

    }
}
