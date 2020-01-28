using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DailyForecaster.Models
{
    public class FinPlannerContext : DbContext
    {
        public FinPlannerContext(DbContextOptions<FinPlannerContext> options)
            : base(options)
        {
        }
        public DbSet<CFType> CFTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CFType>().ToTable("CFType");
        }
    }
}
