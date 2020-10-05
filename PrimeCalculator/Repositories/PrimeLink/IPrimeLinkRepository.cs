using System.Threading.Tasks;
using PrimeCalculator.Models;

namespace PrimeCalculator.Repositories.PrimeLink
{
    public interface IPrimeLinkRepository
    {
        Task<PrimeLinkModel> GetPrimeLinkbyNumberAsync(int number);
        Task StartNewPrimeLinkCalculationAsync(int number);
        Task UpdatePrimeLinkAsync(PrimeLinkModel newPrimeLink);
    }
}