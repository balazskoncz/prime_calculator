using MediatR;
using PrimeCalculator.Models;
using PrimeCalculator.Queries.Base;

namespace PrimeCalculator.Queries
{
    public class GetCalculationStatesByNumberQuery : AbstractNumericQuery, IRequest<CalculationModel>
    {
    }
}
