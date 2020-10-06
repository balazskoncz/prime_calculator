using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PrimeCalculator.Helpers;

namespace PrimeCalculator.HealthChecks
{
    public class DatabaseHealthChecks : IHealthCheck
    {
        private readonly ConnectionStrings _connectionStrings;
        public DatabaseHealthChecks(ConnectionStrings connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            using (var ping = new Ping())
            {
                var pingResult = await ping.SendPingAsync(_connectionStrings.DatabaseServiceHost);

                if (pingResult.Status == IPStatus.Success)
                {
                    return HealthCheckResult.Healthy();
                }

                return HealthCheckResult.Unhealthy();
            }
        }
    }
}
