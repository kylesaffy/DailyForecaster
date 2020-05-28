using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DailyForecaster.Models
{
    public class FinPlannerContext : DbContext
    {
        public FinPlannerContext(DbContextOptions<FinPlannerContext> options) : base(options){ }
        public DbSet<CFType> CFTypes { get; set; }
        public DbSet<CFClassification> CFClassifications { get; set; }
        //public DbSet<CashFlowItem> CashFlowItems { get; set; }
        public DbSet<ManualCashFlow> ManualCashFlows { get; set; }
        public DbSet<AutomatedCashFlow> AutomatedCashFlows { get; set; }
        public DbSet<ReportedTransaction> ReportedTransactions { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<Collections> Collections { get; set; }
        public DbSet<UserCollectionMapping> UserCollectionMapping { get; set; }
        public DbSet<Institution> Institution { get; set; }
		public DbSet<AccountCollectionsMapping> AccountCollectionsMapping { get; set; }
        public DbSet<Budget> Budget { get; set; }
       
    }
}
