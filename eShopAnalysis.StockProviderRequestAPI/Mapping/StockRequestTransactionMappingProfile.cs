using AutoMapper;
using eShopAnalysis.StockProviderRequestAPI.Dto;
using eShopAnalysis.StockProviderRequestAPI.Models;

namespace eShopAnalysis.StockProviderRequestAPI.Mapping
{
    public class StockRequestTransactionMappingProfile: Profile
    {
        public StockRequestTransactionMappingProfile()
        {
            CreateMap<StockRequestTransaction, StockRequestTransactionDto>().ReverseMap();
        }
    }
}
