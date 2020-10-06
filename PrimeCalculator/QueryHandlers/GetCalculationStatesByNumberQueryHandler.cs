using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using PrimeCalculator.Models;
using PrimeCalculator.Queries;
using PrimeCalculator.Repositories;

namespace PrimeCalculator.QueryHandlers
{
    public class GetCalculationStatesByNumberQueryHandler : IRequestHandler<GetCalculationStateByNumberQuery, CalculationModel>
    {
        private readonly ICalculationRepository _calculationRepository;
        private readonly IMapper _mapper;

        public GetCalculationStatesByNumberQueryHandler(
            IMapper mapper,
            ICalculationRepository calculationRepository)
        {
            _mapper = mapper;
            _calculationRepository = calculationRepository;
        }

        public async Task<CalculationModel> Handle(GetCalculationStateByNumberQuery request, CancellationToken cancellationToken)
        {
            var calculation = await _calculationRepository.GetCalculationByNumberAsync(request.Number);

            return calculation ?? _mapper.Map<CalculationModel>(calculation);
        }
    }
}
