using MediatR;

namespace PrimeCalculator.Commands.Base
{
    public abstract class AbstractPrimeCalculationCommand<TCommandResult> : AbstractNumericCommand, IRequest<TCommandResult>
    {
    }
}
