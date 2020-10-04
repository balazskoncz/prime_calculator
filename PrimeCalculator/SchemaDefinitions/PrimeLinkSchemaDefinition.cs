using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrimeCalculator.Entities;

namespace PrimeCalculator.SchemaDefinitions
{
    public class PrimeLinkSchemaDefinition : IEntityTypeConfiguration<PrimeLink>
    {
        public void Configure(EntityTypeBuilder<PrimeLink> builder)
        {
            builder.ToTable("PrimeLink", PrimeDbContext.DEFAULT_SCHEMA);

            builder.HasKey(primeLink => primeLink.Prime);

            builder.Property(primeLink => primeLink.CalculationStatusId).IsRequired();
        }
    }
}
