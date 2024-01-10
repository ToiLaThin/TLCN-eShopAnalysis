using AutoMapper;
using eShopAnalysis.ProductInteractionAPI.Dto;
using eShopAnalysis.ProductInteractionAPI.Models;

namespace eShopAnalysis.ProductInteractionAPI.Mapping
{
    public class LikeMappingProfile: Profile
    {
        public LikeMappingProfile()
        {
            CreateMap<Like, LikeDto>().ReverseMap();
        }
    }
}
