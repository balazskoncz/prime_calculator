using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using PrimeCalculator.Models;
using PrimeCalculator.Queries;
using PrimeCalculator.Repositories.PrimeLink;

namespace PrimeCalculator.QueryHandlers
{
    public class GetPrimeLinkByNumberQueryHandler : IRequestHandler<GetPrimeLinkByNumberQuery, PrimeLinkModel>
    {
        private readonly IPrimeLinkRepository _primeLinkRepository;
        private readonly IMapper _mapper;

        public GetPrimeLinkByNumberQueryHandler(
            IPrimeLinkRepository primeLinkRepository,
            IMapper mapper)
        {
            _mapper = mapper;
            _primeLinkRepository = primeLinkRepository;
        }

        public async Task<PrimeLinkModel> Handle(GetPrimeLinkByNumberQuery request, CancellationToken cancellationToken)
        {
            var primeLink = await _primeLinkRepository.GetPrimeLinkbyNumberAsync(request.Number);

            return primeLink ?? _mapper.Map<PrimeLinkModel>(primeLink);
        }
    }
}
