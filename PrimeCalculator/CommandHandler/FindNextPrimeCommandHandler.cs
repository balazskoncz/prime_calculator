using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PrimeCalculator.CommandResults;
using PrimeCalculator.Commands;

namespace PrimeCalculator.CommandHandler
{
    public class FindNextPrimeCommandHandler : IRequestHandler<FindNextPrimeCommand, FindNextPrimeCommandResult>
    {
        public Task<FindNextPrimeCommandResult> Handle(FindNextPrimeCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new FindNextPrimeCommandResult { NextPrime = 42 });
        }
    }
}
