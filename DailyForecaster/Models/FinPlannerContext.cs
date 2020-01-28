using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DailyForecaster.Models
{
    public class FinPlannerContext : DbContext
    {
        public FinPlannerContext(DbContextOptions<FinPlannerContext> options) : base(options){ }
        public DbSet<CFType> CFTypes { get; set; }
        public DbSet<CFClassification> CFClassifications { get; set; }
        public DbSet<CashFlowItem> CashFlowItems { get; set; }
        public DbSet<ManualCashFlow> CashFlows { get; set; }
       
    }
}
