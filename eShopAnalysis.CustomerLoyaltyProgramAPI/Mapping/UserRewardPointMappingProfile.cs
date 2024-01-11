using AutoMapper;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Dto;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Models;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.Mapping
{
    public class UserRewardPointMappingProfile: Profile
    {
        public UserRewardPointMappingProfile()
        {
            CreateMap<UserRewardPoint, UserRewardPointDto>().ReverseMap();
        }
    }
}
