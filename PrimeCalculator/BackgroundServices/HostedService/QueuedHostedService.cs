using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using PrimeCalculator.BackgroundServices.Queue;

namespace PrimeCalculator.BackgroundServices.HostedService
{
    public class QueuedHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        public IBackgroundTaskQueue TaskQueue { get; }

        public QueuedHostedService(
            IBackgroundTaskQueue taskQueue,
            IServiceProvider serviceProvider)
        {
            TaskQueue = taskQueue;
            _serviceProvider = serviceProvider;
            //TODO: log
            //_logger = loggerFactory.CreateLogger<QueuedHostedService>();
        }


        protected async override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            //_logger.LogInformation("Queued Hosted Service is starting.");

            while (!cancellationToken.IsCancellationRequested)
            {
                var workItem = await TaskQueue.DequeueAsync(cancellationToken);

                try
                {
                    await workItem(cancellationToken, _serviceProvider);
                }
                catch (Exception ex)
                {
                    //_logger.LogError(ex,
                    //   "Error occurred executing {WorkItem}.", nameof(workItem));
                }
            }

            //_logger.LogInformation("Queued Hosted Service is stopping.");
        }
    }
}
