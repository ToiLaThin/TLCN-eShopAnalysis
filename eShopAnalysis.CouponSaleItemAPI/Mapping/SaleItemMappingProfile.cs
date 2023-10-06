using AutoMapper;
using eShopAnalysis.CouponSaleItemAPI.Dto;
using eShopAnalysis.CouponSaleItemAPI.Models;

namespace eShopAnalysis.CouponSaleItemAPI.Mapping
{
    public class SaleItemMappingProfile : Profile
    {
        public SaleItemMappingProfile()
        {
            CreateMap<SaleItem, SaleItemDto>().ReverseMap();
        }
    }
}
