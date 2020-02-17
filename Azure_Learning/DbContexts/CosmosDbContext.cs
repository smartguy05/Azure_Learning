using AzureLearning.Models;
using Microsoft.EntityFrameworkCore;

namespace AzureLearning.DbContexts
{
    public class CosmosDbContext : DbContext
    {
        private readonly AppSettings _settings;

        public CosmosDbContext(AppSettings settings)
        {
            _settings = settings;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseCosmos(
                _settings.CosmosDb.Uri,
                _settings.CosmosDb.Key,
                _settings.CosmosDb.DatabaseName);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasDefaultContainer(_settings.CosmosDb.CollectionName)
                .Entity<EmployeeEntity>()
                .HasNoDiscriminator();

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<EmployeeEntity> Employees { get; set; }
    }
}