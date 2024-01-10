using AutoMapper;
using eShopAnalysis.ProductInteractionAPI.Dto;
using eShopAnalysis.ProductInteractionAPI.Models;

namespace eShopAnalysis.ProductInteractionAPI.Mapping
{
    public class RateMappingProfile: Profile
    {
        public RateMappingProfile()
        {
            CreateMap<Rate, RateDto>().ReverseMap();
        }
    }
}
