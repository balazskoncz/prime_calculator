using MediatR;

namespace PrimeCalculator.Commands.Base
{
    public abstract class AbstractPrimeCalculationCommand<TCommandResult> : IRequest<TCommandResult>
    {
        public int Number { get; set; }
    }
}
