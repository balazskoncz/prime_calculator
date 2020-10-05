using AutoMapper;
using PrimeCalculator.Entities;
using PrimeCalculator.Models;

namespace PrimeCalculator.MapperProfiles
{
    public class CalculationProfile : Profile
    {
        public CalculationProfile()
        {
            CreateMap<Calculation, CalculationModel>().ReverseMap();
        }
    }
}
