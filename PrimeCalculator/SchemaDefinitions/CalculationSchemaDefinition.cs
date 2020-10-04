using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrimeCalculator.Entities;

namespace PrimeCalculator.SchemaDefinitions
{
    public class CalculationSchemaDefinition : IEntityTypeConfiguration<Calculation>
    {
        public void Configure(EntityTypeBuilder<Calculation> builder)
        {
            builder.ToTable("Calculation", PrimeDbContext.DEFAULT_SCHEMA);

            builder.HasKey(calculation => calculation.Number);

            builder.Property(calculation => calculation.CalculationStatusId).IsRequired();
        }
    }
}
