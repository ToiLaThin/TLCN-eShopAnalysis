using AutoMapper;
using eShopAnalysis.CouponSaleItemAPI.Dto;
using eShopAnalysis.CouponSaleItemAPI.Models;

namespace eShopAnalysis.CouponSaleItemAPI.Mapping
{
    public class CouponMappingProfile : Profile
    {
        public CouponMappingProfile()
        {
            CreateMap<Coupon, CouponDto>().ReverseMap();
        }
    }
}
