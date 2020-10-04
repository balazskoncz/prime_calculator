using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using PrimeCalculator.CommandHandler.Base;
using PrimeCalculator.CommandResults;
using PrimeCalculator.Commands;
using ScienceInterface.Generated;

namespace PrimeCalculator.CommandHandler
{
    public class CheckNumberIsPrimeCommandHandler : AbstractScienceCommandHandler<CheckNumberIsPrimeCommand, CheckNumberIsPrimeCommandResult>
    {
        public async override Task<CheckNumberIsPrimeCommandResult> Handle(CheckNumberIsPrimeCommand request, CancellationToken cancellationToken)
        {
            var scienceRequestData = new PrimeCalculationRequest
            {
                Number = request.Number
            };

            var scienceReply = await UseScienceInterface("NumberIsPrime", scienceRequestData.ToByteArray(), cancellationToken);

            var scienceResult = NumberIsPrimeResponse.Parser.ParseFrom(scienceReply.RawBytes);

            return new CheckNumberIsPrimeCommandResult
            {
                IsPrime = scienceResult.IsPrime
            };
        }
    }
}
