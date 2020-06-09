using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;

namespace DailyForecaster.Models
{
    public partial class FinPlannerContext : DbContext
    {
        public FinPlannerContext(DbContextOptions<FinPlannerContext> options) : base(options){ }
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
