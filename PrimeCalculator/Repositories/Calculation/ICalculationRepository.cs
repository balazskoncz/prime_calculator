using System.Threading.Tasks;
using PrimeCalculator.Models;

namespace PrimeCalculator.Repositories
{
    public interface ICalculationRepository
    {
        Task<CalculationModel> GetCalculationByNumberAsync(int number);
        Task StartNewCalculationAsync(int number);
        Task UpdateCalculationAsync(CalculationModel newCalculation);
    }
}