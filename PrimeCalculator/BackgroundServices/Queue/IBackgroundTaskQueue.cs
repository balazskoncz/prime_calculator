using System;
using System.Threading;
using System.Threading.Tasks;

namespace PrimeCalculator.BackgroundServices.Queue
{
    /// <summary>
    /// Implements BackgroundTask Queue from msdn:
    /// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-2.1&tabs=visual-studio#queued-background-tasks-1
    /// </summary>
    public interface IBackgroundTaskQueue
    {
        void QueueBackgroundWorkItem(Func<CancellationToken, IServiceProvider, Task> workItem);

        Task<Func<CancellationToken, IServiceProvider, Task>> DequeueAsync(CancellationToken cancellationToken);
    }
}
