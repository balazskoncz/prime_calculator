using MediatR;
using PrimeCalculator.Models;
using PrimeCalculator.Queries.Base;

namespace PrimeCalculator.Queries
{
    public class GetPrimeLinkByNumberQuery : AbstractNumericQuery, IRequest<PrimeLinkModel>
    {
    }
}
