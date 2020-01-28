using Microsoft.EntityFrameworkCore;

namespace TodoApi.Models
{
    public class FinPlannerContext : DbContext
    {
        public FinPlannerContext(DbContextOptions<FinPlannerContext> options)
            : base(options)
        {
        }

        public DbSet<FinPlannerContext> TodoItems { get; set; }
    }
}