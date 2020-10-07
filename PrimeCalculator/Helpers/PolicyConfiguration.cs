namespace PrimeCalculator.Helpers
{
    public class PolicyConfiguration
    {
        public int CircuitBreakerAllowedExceptions { get; set; }

        public double CircuitBreakerShutdownDurationInMinutes { get; set; }

        public int TimoutInMinutes { get; set; }
    }
}
