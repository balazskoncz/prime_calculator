using AutoMapper;
using PrimeCalculator.Dtos;
using PrimeCalculator.Entities;
using PrimeCalculator.Models;
using PrimeCalculator.TypeSafeEnums;

namespace PrimeCalculator.MapperProfiles
{
    public class PrimeLinkProfile : Profile
    {
        public PrimeLinkProfile()
        {
            CreateMap<PrimeLink, PrimeLinkModel>()
                .ForMember(destination => destination.CalculationStatus, options => options.MapFrom(source => CalculationStatus.GetStatusFromId(source.CalculationStatusId)));

            CreateMap<PrimeLinkModel, PrimeLinkDto>()
                .ForMember(destination => destination.NextPrime, options =>
                {
                    options.PreCondition(source => source.CalculationStatus == CalculationStatus.Done);
                    options.MapFrom(source => source.NextPrime);
                });
        }
    }
}
