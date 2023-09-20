using AutoMapper;
using eShopAnalysis.ProductCatalogAPI.Application.Dto;
using eShopAnalysis.ProductCatalogAPI.Domain.Models;

namespace eShopAnalysis.ProductCatalogAPI.Application.Mapping
{
    public class CatalogMappingProfile : Profile
    {
        public CatalogMappingProfile() {
            CreateMap<Catalog, CatalogDto>().ReverseMap();
        }
    }
}
