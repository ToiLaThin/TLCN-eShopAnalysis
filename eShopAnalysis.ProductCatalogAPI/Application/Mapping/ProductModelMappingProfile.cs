using AutoMapper;
using eShopAnalysis.ProductCatalogAPI.Application.Dto;
using eShopAnalysis.ProductCatalogAPI.Domain.Models;

namespace eShopAnalysis.ProductCatalogAPI.Application.Mapping
{
    public class ProductModelMappingProfile : Profile
    {
        public ProductModelMappingProfile()
        {
            CreateMap<ProductModel, ProductModelDto>().ReverseMap();
        }
    }
}
