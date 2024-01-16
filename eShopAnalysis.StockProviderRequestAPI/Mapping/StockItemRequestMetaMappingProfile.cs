using AutoMapper;
using eShopAnalysis.StockProviderRequestAPI.Dto;
using eShopAnalysis.StockProviderRequestAPI.Models;

namespace eShopAnalysis.StockProviderRequestAPI.Mapping
{
    public class StockItemRequestMetaMappingProfile: Profile
    {
        public StockItemRequestMetaMappingProfile() { 
            CreateMap<StockItemRequestMeta, StockItemRequestMetaDto>().ReverseMap();
        }
    }
}
