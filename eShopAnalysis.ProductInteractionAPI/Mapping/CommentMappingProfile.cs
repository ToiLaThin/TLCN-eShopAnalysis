using AutoMapper;
using eShopAnalysis.ProductInteractionAPI.Dto;
using eShopAnalysis.ProductInteractionAPI.Models;

namespace eShopAnalysis.ProductInteractionAPI.Mapping
{
    public class CommentMappingProfile: Profile
    {
        public CommentMappingProfile()
        {
            CreateMap<Comment, CommentDto>().ReverseMap();
        }
    }
}
