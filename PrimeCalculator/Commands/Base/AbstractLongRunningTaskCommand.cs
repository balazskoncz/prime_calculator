using MediatR;

namespace PrimeCalculator.Commands.Base
{
    public abstract class AbstractLongRunningTaskCommand : AbstractNumericCommand, IRequest
    {
    }
}
