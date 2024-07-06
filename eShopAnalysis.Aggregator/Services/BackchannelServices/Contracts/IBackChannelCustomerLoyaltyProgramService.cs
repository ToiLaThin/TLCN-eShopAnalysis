using eShopAnalysis.Aggregator.Result;
using eShopAnalysis.Aggregator.Services.BackchannelDto;
using Microsoft.AspNetCore.Mvc;

namespace eShopAnalysis.Aggregator.Services.BackchannelServices
{
    public interface IBackChannelCustomerLoyaltyProgramService
    {
        Task<BackChannelResponseDto<RewardTransactionDto>> AddRewardTransactionForApplyCoupon([FromBody] RewardTransactionForApplyCouponAddRequestDto requestDto);
        Task<BackChannelResponseDto<RewardTransactionDto>> AddRewardTransactionForCompleteOrdering([FromBody] RewardTransactionForCompleteOrderingAddRequestDto requestDto);

        int ConvertOrderPriceToRewardPoint(double orderPrice);
    }
}
