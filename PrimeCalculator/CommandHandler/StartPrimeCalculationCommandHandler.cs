using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PrimeCalculator.BackgroundServices.Queue;
using PrimeCalculator.CommandHandler.Base;
using PrimeCalculator.Commands;
using PrimeCalculator.Helpers;
using PrimeCalculator.Models;
using PrimeCalculator.Repositories;
using PrimeCalculator.TypeSafeEnums;
using ScienceInterface.Generated;

namespace PrimeCalculator.CommandHandler
{
    public class StartPrimeCalculationCommandHandler : AbstractScienceCommandHandler, IRequestHandler<StartPrimeCalculationCommand>
    {
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;

        public StartPrimeCalculationCommandHandler(
            IBackgroundTaskQueue backgroundTaskQueue,
            ConnectionStrings connectionStrings) : base(connectionStrings)
        {
            _backgroundTaskQueue = backgroundTaskQueue;
        }

        public Task<Unit> Handle(StartPrimeCalculationCommand request, CancellationToken cancellationToken)
        {
            var scienceRequestData = new PrimeCalculationRequest
            {
                Number = request.Number
            };

            _backgroundTaskQueue.QueueBackgroundWorkItem(async (token, serviceProvider) =>
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var calculationRepository = scope.ServiceProvider.GetRequiredService<ICalculationRepository>();

                    var calculation = await calculationRepository.GetCalculationByNumberAsync(request.Number);

                    if (calculation == null
                    || calculation.CalculationStatus == CalculationStatus.Failed)
                    {
                        if (calculation == null)
                        {
                            await calculationRepository.StartNewCalculationAsync(request.Number);
                        }

                        var scienceReply = await UseScienceInterface("NumberIsPrime", scienceRequestData.ToByteArray(), token);

                        var scienceResult = NumberIsPrimeResponse.Parser.ParseFrom(scienceReply.RawBytes);

                        await calculationRepository.UpdateCalculationAsync(new CalculationModel
                        {
                            Number = request.Number,
                            CalculationStatus = CalculationStatus.Done,
                            IsPrime = scienceResult.IsPrime
                        });
                    }
                }
            });

            // :(
            return Task.FromResult(new Unit());
        }
    }
}
