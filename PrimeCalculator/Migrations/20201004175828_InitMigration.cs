using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace PrimeCalculator.Migrations
{
    public partial class InitMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "primes");

            migrationBuilder.CreateTable(
                name: "Calculation",
                schema: "primes",
                columns: table => new
                {
                    Number = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CalculationStatusId = table.Column<int>(nullable: false),
                    IsPrime = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calculation", x => x.Number);
                });

            migrationBuilder.CreateTable(
                name: "PrimeLink",
                schema: "primes",
                columns: table => new
                {
                    Prime = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CalculationStatusId = table.Column<int>(nullable: false),
                    NextPrime = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrimeLink", x => x.Prime);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Calculation",
                schema: "primes");

            migrationBuilder.DropTable(
                name: "PrimeLink",
                schema: "primes");
        }
    }
}
