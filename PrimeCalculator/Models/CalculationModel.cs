﻿using PrimeCalculator.TypeSafeEnums;

namespace PrimeCalculator.Models
{
    public class CalculationModel
    {
        public int Number { get; set; }

        public CalculationStatus CalculationStatus { get; set; }

        public bool IsPrime { get; set; }
    }
}
