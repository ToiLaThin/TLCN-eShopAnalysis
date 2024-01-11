using AutoMapper;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Dto;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Models;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.Mapping
{
    public class RewardTransactionMappingProfile: Profile
    {
        public RewardTransactionMappingProfile() {
            CreateMap<RewardTransaction, RewardTransactionDto>().ReverseMap();
        }
    }
}
