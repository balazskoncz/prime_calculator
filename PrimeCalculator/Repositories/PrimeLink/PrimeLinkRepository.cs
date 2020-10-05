using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PrimeCalculator.Models;
using PrimeCalculator.TypeSafeEnums;

namespace PrimeCalculator.Repositories.PrimeLink
{
    public class PrimeLinkRepository : IPrimeLinkRepository
    {
        private readonly PrimeDbContext _primeDbContext;
        private readonly IMapper _mapper;

        public PrimeLinkRepository(
            PrimeDbContext primeDbContext,
            IMapper mapper)
        {
            _mapper = mapper;
            _primeDbContext = primeDbContext;
        }

        public async Task<PrimeLinkModel> GetPrimeLinkbyNumberAsync(int number)
        {
            var row = await _primeDbContext.PrimeLinks.AsNoTracking().FirstOrDefaultAsync(primeLink => primeLink.Number == number);

            if (row == null)
            {
                return null;
            }

            return _mapper.Map<PrimeLinkModel>(row);
        }


        public async Task StartNewPrimeLinkCalculationAsync(int number)
        {
            var primeLinkCalculation = new Entities.PrimeLink
            {
                Number = number,
                CalculationStatusId = CalculationStatus.InProgress.StatusId,
            };

            await _primeDbContext.PrimeLinks.AddAsync(primeLinkCalculation);
            await _primeDbContext.SaveChangesAsync();
        }


        public async Task UpdatePrimeLinkAsync(PrimeLinkModel newPrimeLink)
        {
            var entity = await _primeDbContext.PrimeLinks.FirstOrDefaultAsync(primeLink => primeLink.Number == newPrimeLink.Number);

            entity.CalculationStatusId = newPrimeLink.CalculationStatusId;
            entity.NextPrime = newPrimeLink.NextPrime;

            await _primeDbContext.SaveChangesAsync();
        }
    }
}
