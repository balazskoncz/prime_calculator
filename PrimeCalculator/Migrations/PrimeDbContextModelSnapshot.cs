﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PrimeCalculator;

namespace PrimeCalculator.Migrations
{
    [DbContext(typeof(PrimeDbContext))]
    partial class PrimeDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("PrimeCalculator.Entities.Calculation", b =>
                {
                    b.Property<int>("Number")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("CalculationStatusId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsPrime")
                        .HasColumnType("boolean");

                    b.HasKey("Number");

                    b.ToTable("Calculation","primes");
                });

            modelBuilder.Entity("PrimeCalculator.Entities.PrimeLink", b =>
                {
                    b.Property<int>("Prime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("CalculationStatusId")
                        .HasColumnType("integer");

                    b.Property<int>("NextPrime")
                        .HasColumnType("integer");

                    b.HasKey("Prime");

                    b.ToTable("PrimeLink","primes");
                });
#pragma warning restore 612, 618
        }
    }
}
