using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PrimeCalculator.Entities;
using PrimeCalculator.Models;
using PrimeCalculator.TypeSafeEnums;

namespace PrimeCalculator.Repositories
{
    public class CalculationRepository : ICalculationRepository
    {
        private readonly PrimeDbContext _primeDbContext;
        private readonly IMapper _mapper;

        public CalculationRepository(
            PrimeDbContext primeDbContext,
            IMapper mapper)
        {
            _mapper = mapper;
            _primeDbContext = primeDbContext;
        }

        public async Task<CalculationModel> GetCalculationByNumberAsync(int number)
        {
            var row = await _primeDbContext.Calculations.AsNoTracking().FirstOrDefaultAsync(
                calculation => calculation.Number == number);

            if (row == null)
            {
                return null;
            }

            return _mapper.Map<CalculationModel>(row);
        }

        public async Task StartNewCalculationAsync(int number)
        {
            var entity = new Calculation
            {
                Number = number,
                CalculationStatusId = CalculationStatus.InProgress.StatusId
            };

            _primeDbContext.Add(entity);
            await _primeDbContext.SaveChangesAsync();
        }

        public async Task UpdateCalculationAsync(CalculationModel newCalculation)
        {
            var row = await _primeDbContext.Calculations.FirstOrDefaultAsync(
                calculation => calculation.Number == newCalculation.Number);

            if (row == null)
            {
                return;
            }

            row.IsPrime = newCalculation.IsPrime;
            row.CalculationStatusId = newCalculation.CalculationStatus.StatusId;

            await _primeDbContext.SaveChangesAsync();
        }
    }
}
