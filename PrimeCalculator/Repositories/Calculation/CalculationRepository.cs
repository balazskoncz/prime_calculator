using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PrimeCalculator.Entities;
using PrimeCalculator.Models;
using PrimeCalculator.TypeSafeEnums;

namespace PrimeCalculator.Repositories
{
    public class CalculationRepository : ICalculationRepository
    {
        private readonly PrimeDbContext _primeDbContext;

        public CalculationRepository(PrimeDbContext primeDbContext)
        {
            _primeDbContext = primeDbContext;
        }

        public async Task<CalculationModel> GetCalculationByNumber(int number)
        {
            var row = await _primeDbContext.Calculations.AsNoTracking().FirstOrDefaultAsync(
                calculation => calculation.Number == number
                && calculation.CalculationStatusId == CalculationStatus.Done.StatusId);

            if (row == null)
            {
                return null;
            }

            return new CalculationModel 
            {
                Number = row.Number,
                CalculationStatusId = row.CalculationStatusId,
                IsPrime = row.IsPrime
            };
        }

        public async Task StartNewCalculation(int number)
        {
            var entity = new Calculation
            {
                Number = number,
                CalculationStatusId = CalculationStatus.InProgress.StatusId
            };

            _primeDbContext.Add(entity);
            await _primeDbContext.SaveChangesAsync();
        }

        public async Task UpdateCalculation(CalculationModel newCalculation)
        {
            var row = await _primeDbContext.Calculations.FirstOrDefaultAsync(
                calculation => calculation.Number == newCalculation.Number);

            if (row == null)
            {
                return;
            }

            row.IsPrime = newCalculation.IsPrime;
            row.CalculationStatusId = newCalculation.CalculationStatusId;

            await _primeDbContext.SaveChangesAsync();
        }
    }
}
