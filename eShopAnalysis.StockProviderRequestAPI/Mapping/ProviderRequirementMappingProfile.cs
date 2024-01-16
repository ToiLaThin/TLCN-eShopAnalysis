using AutoMapper;
using eShopAnalysis.StockProviderRequestAPI.Dto;
using eShopAnalysis.StockProviderRequestAPI.Models;

namespace eShopAnalysis.StockProviderRequestAPI.Mapping
{
    public class ProviderRequirementMappingProfile: Profile
    {
        public ProviderRequirementMappingProfile()
        {
            CreateMap<ProviderRequirement, ProviderRequirementDto>().ReverseMap();
        }
    }
}
