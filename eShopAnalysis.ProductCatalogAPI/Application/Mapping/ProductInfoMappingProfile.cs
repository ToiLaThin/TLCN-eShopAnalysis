using AutoMapper;
using eShopAnalysis.ProductCatalogAPI.Application.Dto;
using eShopAnalysis.ProductCatalogAPI.Domain.Models;

namespace eShopAnalysis.ProductCatalogAPI.Application.Mapping
{
    public class ProductInfoMappingProfile : Profile
    {
        public ProductInfoMappingProfile()
        {
            CreateMap<ProductInfo, ProductInfoDto>().ReverseMap();
        }
    }
}
