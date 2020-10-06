using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PrimeCalculator.BackgroundServices.Queue;
using PrimeCalculator.CommandHandler.Base;
using PrimeCalculator.Commands;
using PrimeCalculator.Helpers;
using PrimeCalculator.Repositories.PrimeLink;
using PrimeCalculator.TypeSafeEnums;
using ScienceInterface.Generated;

namespace PrimeCalculator.CommandHandler
{
    public class StartNextPrimeCalculationCommandHandler : AbstractScienceCommandHandler, IRequestHandler<StartNextPrimeCalculationCommand>
    {
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;

        public StartNextPrimeCalculationCommandHandler(
            IBackgroundTaskQueue backgroundTaskQueue,
            ConnectionStrings connectionStrings) : base(connectionStrings)
        {
            _backgroundTaskQueue = backgroundTaskQueue;
        }

        public Task<Unit> Handle(StartNextPrimeCalculationCommand request, CancellationToken cancellationToken)
        {
            var scienceRequestData = new PrimeCalculationRequest
            {
                Number = request.Number
            };

            _backgroundTaskQueue.QueueBackgroundWorkItem(async (token, serviceProvider) =>
            {
                using (var scope = serviceProvider.CreateScope())
                { 
                    var primeLinkRepository = scope.ServiceProvider.GetRequiredService<IPrimeLinkRepository>();

                    var primeLink = await primeLinkRepository.GetPrimeLinkbyNumberAsync(request.Number);

                    if (primeLink == null
                        || primeLink.CalculationStatus == CalculationStatus.Failed)
                    {
                        if (primeLink == null)
                        {
                            await primeLinkRepository.StartNewPrimeLinkCalculationAsync(request.Number);
                        }

                        var scienceReply = await UseScienceInterface("FindNextPrime", scienceRequestData.ToByteArray(), cancellationToken);

                        var scienceResult = NextPrimeResponse.Parser.ParseFrom(scienceReply.RawBytes);

                        await primeLinkRepository.UpdatePrimeLinkAsync(new Models.PrimeLinkModel
                        {
                            Number = request.Number,
                            CalculationStatus = CalculationStatus.Done,
                            NextPrime = scienceResult.NextPrime
                        });
                    }
                }
            });


            return Task.FromResult(new Unit());
        }
    }
}
