using AutoMapper;
using eShopAnalysis.StockProviderRequestAPI.Dto;
using eShopAnalysis.StockProviderRequestAPI.Models;

namespace eShopAnalysis.StockProviderRequestAPI.Mapping
{
    public class StockItemRequestMappingProfile: Profile
    {
        public StockItemRequestMappingProfile()
        {
            CreateMap<StockItemRequest, StockItemRequestDto>().ReverseMap();
        }
    }
}
