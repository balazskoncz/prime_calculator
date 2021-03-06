﻿using PrimeCalculator.TypeSafeEnums;

namespace PrimeCalculator.Models
{
    public class PrimeLinkModel
    {
        public int Number { get; set; }

        public CalculationStatus CalculationStatus { get; set; }

        public int NextPrime { get; set; }
    }
}
