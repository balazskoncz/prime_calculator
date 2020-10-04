using Microsoft.EntityFrameworkCore;
using PrimeCalculator.Entities;
using PrimeCalculator.SchemaDefinitions;

namespace PrimeCalculator
{
    public class PrimeDbContext : DbContext
    {
        public const string DEFAULT_SCHEMA = "primes";

        public virtual DbSet<Calculation> Calculations { get; set; }

        public virtual DbSet<PrimeLink> PrimeLinks { get; set; }

        public PrimeDbContext(DbContextOptions<PrimeDbContext> context)
            : base(context)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CalculationSchemaDefinition());
            modelBuilder.ApplyConfiguration(new PrimeLinkSchemaDefinition());

            base.OnModelCreating(modelBuilder);
        }
    }
}
