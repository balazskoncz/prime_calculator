using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using PrimeCalculator.CommandHandler.Base;
using PrimeCalculator.CommandResults;
using PrimeCalculator.Commands;
using ScienceInterface.Generated;

namespace PrimeCalculator.CommandHandler
{
    public class FindNextPrimeCommandHandler : AbstractScienceCommandHandler<FindNextPrimeCommand, FindNextPrimeCommandResult>
    {
        public async override Task<FindNextPrimeCommandResult> Handle(FindNextPrimeCommand request, CancellationToken cancellationToken)
        {
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
