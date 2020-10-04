using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using PrimeCalculator.CommandHandler.Base;
using PrimeCalculator.CommandResults;
using PrimeCalculator.Commands;
using PrimeCalculator.Repositories;
using ScienceInterface.Generated;

namespace PrimeCalculator.CommandHandler
{
    public class FindNextPrimeCommandHandler : AbstractScienceCommandHandler<FindNextPrimeCommand, FindNextPrimeCommandResult>
    {
        private readonly ICalculationRepository _calculationRepository;

        public FindNextPrimeCommandHandler(ICalculationRepository calculationRepository)
        {
            _calculationRepository = calculationRepository;
        }

        public async override Task<FindNextPrimeCommandResult> Handle(FindNextPrimeCommand request, CancellationToken cancellationToken)
        {
            await _calculationRepository.StartNewCalculation(request.Number);

            var scienceRequestData = new PrimeCalculationRequest
            {
                Number = request.Number
            };

            var scienceReply = await UseScienceInterface("FindNextPrime", scienceRequestData.ToByteArray(), cancellationToken);

            var scienceResult = NextPrimeResponse.Parser.ParseFrom(scienceReply.RawBytes);

            return new FindNextPrimeCommandResult 
            { 
                NextPrime = scienceResult.NextPrime
            };
        }
    }
}
