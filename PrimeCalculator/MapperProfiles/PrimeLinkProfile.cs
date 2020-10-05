using AutoMapper;
using PrimeCalculator.Entities;
using PrimeCalculator.Models;

namespace PrimeCalculator.MapperProfiles
{
    public class PrimeLinkProfile : Profile
    {
        public PrimeLinkProfile()
        {
            CreateMap<PrimeLink, PrimeLinkModel>().ReverseMap();
        }
    }
}
