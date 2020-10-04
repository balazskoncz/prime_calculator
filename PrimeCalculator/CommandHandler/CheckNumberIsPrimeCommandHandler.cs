using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PrimeCalculator.CommandResults;
using PrimeCalculator.Commands;

namespace PrimeCalculator.CommandHandler
{
    public class CheckNumberIsPrimeCommandHandler : IRequestHandler<CheckNumberIsPrimeCommand, CheckNumberIsPrimeCommandResult>
    {
        public Task<CheckNumberIsPrimeCommandResult> Handle(CheckNumberIsPrimeCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new CheckNumberIsPrimeCommandResult { IsPrime = true });
        }
    }
}
