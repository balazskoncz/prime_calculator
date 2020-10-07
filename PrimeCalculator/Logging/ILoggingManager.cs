namespace PrimeCalculator.Logging
{
    public interface ILoggingManager
    {
        void LogError(string error);
        void LogFatal(string message);
        void LogInformation(string message);
    }
}