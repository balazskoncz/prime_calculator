using PrimeCalculator.TypeSafeEnums;

namespace PrimeCalculator.Dtos
{
    public class PrimeLinkDto
    {
        public int Number { get; set; }

        public CalculationStatus CalculationStatus { get; set; }

        public int? NextPrime { get; set; }
    }
}
