using AzureLearning.Models;
using Microsoft.EntityFrameworkCore;

namespace AzureLearning.DbContexts
{
    public class SqlDbContext : DbContext
    {
        private readonly AppSettings _settings;

        public SqlDbContext(AppSettings settings)
        {
            _settings = settings;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_settings.ConnectionStrings.SqlDb)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SqlEmployee>()
                .ToTable("Employee");
        }

        public DbSet<SqlEmployee> Employees { get; set; }
    }
}