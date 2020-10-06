using PrimeCalculator.TypeSafeEnums;

namespace PrimeCalculator.Dtos
{
    public class CalculationDto
    {
        public int Number { get; set; }

        public CalculationStatus CalculationStatus { get; set; }

        public bool? IsPrime { get; set; }
    }
}
