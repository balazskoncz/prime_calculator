using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PrimeCalculator.Helpers;

namespace PrimeCalculator.HealthChecks
{
    public class ScienceHealthCheck : IHealthCheck
    {
        private readonly ConnectionStrings _connectionStrings;
        public ScienceHealthCheck(ConnectionStrings connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            using (var ping = new Ping())
            {
                var pingResult = await ping.SendPingAsync(_connectionStrings.ScienceServiceHost);

                if (pingResult.Status == IPStatus.Success)
                {
                    return HealthCheckResult.Healthy();
                }

                return HealthCheckResult.Unhealthy();
            }

        }
    }
}
