using AutoMapper;
using PrimeCalculator.Dtos;
using PrimeCalculator.Entities;
using PrimeCalculator.Models;
using PrimeCalculator.TypeSafeEnums;

namespace PrimeCalculator.MapperProfiles
{
    public class CalculationProfile : Profile
    {
        public CalculationProfile()
        {
            CreateMap<Calculation, CalculationModel>()
                .ForMember(destination => destination.CalculationStatus, options => options.MapFrom(source => CalculationStatus.GetStatusFromId(source.CalculationStatusId)));

            CreateMap<CalculationModel, CalculationDto>()
                .ForMember(destination => destination.IsPrime, options => 
                {
                    options.PreCondition(source => source.CalculationStatus == CalculationStatus.Done);
                    options.MapFrom(source => source.IsPrime);
                });
        }
    }
}
