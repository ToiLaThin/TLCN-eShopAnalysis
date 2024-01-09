using AutoMapper;
using eShopAnalysis.ProductInteractionAPI.Dto;
using eShopAnalysis.ProductInteractionAPI.Models;

namespace eShopAnalysis.ProductInteractionAPI.Mapping
{
    public class BookmarkMappingProfile: Profile
    {
        public BookmarkMappingProfile()
        {
            CreateMap<Bookmark, BookmarkDto>().ReverseMap();
        }
    }
}
