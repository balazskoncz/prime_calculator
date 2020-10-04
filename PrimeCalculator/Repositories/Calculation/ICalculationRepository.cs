using System.Threading.Tasks;
using PrimeCalculator.Models;

namespace PrimeCalculator.Repositories
{
    public interface ICalculationRepository
    {
        Task<CalculationModel> GetCalculationByNumber(int number);
        Task StartNewCalculation(int number);
        Task UpdateCalculation(CalculationModel newCalculation);
    }
}