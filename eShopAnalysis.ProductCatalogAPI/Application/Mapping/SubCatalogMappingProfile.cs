using AutoMapper;
using eShopAnalysis.ProductCatalogAPI.Application.Dto;
using eShopAnalysis.ProductCatalogAPI.Domain.Models;

namespace eShopAnalysis.ProductCatalogAPI.Application.Mapping
{
    public class SubCatalogMappingProfile : Profile
    {
        //can map ca subcatalog chu ko rieng gi catalog vi muon map catalog phai co subcatalog
        //AutoMapper.AutoMapperMappingException: Error mapping types.
        //Mapping types:
        //SubCatalog -> SubCatalogDto
        //eShopAnalysis.ProductCatalogAPI.Models.SubCatalog -> eShopAnalysis.ProductCatalogAPI.Models.Dto.SubCatalogDto
        public SubCatalogMappingProfile() {
            CreateMap<SubCatalog, SubCatalogDto>().ReverseMap();
        }
    }
}
