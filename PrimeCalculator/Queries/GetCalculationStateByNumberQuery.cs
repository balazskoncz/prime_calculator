using MediatR;
using PrimeCalculator.Models;
using PrimeCalculator.Queries.Base;

namespace PrimeCalculator.Queries
{
    public class GetCalculationStateByNumberQuery : AbstractNumericQuery, IRequest<CalculationModel>
    {
    }
}
