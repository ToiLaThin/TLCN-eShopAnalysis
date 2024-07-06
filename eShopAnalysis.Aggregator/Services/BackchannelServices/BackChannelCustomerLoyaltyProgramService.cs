using eShopAnalysis.Aggregator.Result;
using eShopAnalysis.Aggregator.Services.BackchannelDto;
using eShopAnalysis.Aggregator.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace eShopAnalysis.Aggregator.Services.BackchannelServices
{
    public class BackChannelCustomerLoyaltyProgramService : IBackChannelCustomerLoyaltyProgramService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IOptions<BackChannelCommunication> _backChannelUrls;
        public BackChannelCustomerLoyaltyProgramService(IServiceProvider serviceProvider, IOptions<BackChannelCommunication> backChannleUrls)
        {
            _backChannelUrls = backChannleUrls;
            _serviceProvider = serviceProvider;
        }
        public async Task<BackChannelResponseDto<RewardTransactionDto>> AddRewardTransactionForApplyCoupon([FromBody] RewardTransactionForApplyCouponAddRequestDto requestDto)
        {
            var baseService = _serviceProvider.GetRequiredService<IBackChannelBaseService<RewardTransactionForApplyCouponAddRequestDto, RewardTransactionDto>>();
            var result = await baseService.SendAsync(new BackChannelRequestDto<RewardTransactionForApplyCouponAddRequestDto>()
            {
                ApiType = ApiType.POST,
                Url = $"{_backChannelUrls.Value.CustomerLoyaltyProgramAPIBaseUri}/AddRewardTransactionForApplyCoupon",
                Data = requestDto
            });
            return result;
        }

        public async Task<BackChannelResponseDto<RewardTransactionDto>> AddRewardTransactionForCompleteOrdering([FromBody] RewardTransactionForCompleteOrderingAddRequestDto requestDto)
        {
            var baseService = _serviceProvider.GetRequiredService<IBackChannelBaseService<RewardTransactionForCompleteOrderingAddRequestDto, RewardTransactionDto>>();
            var result = await baseService.SendAsync(new BackChannelRequestDto<RewardTransactionForCompleteOrderingAddRequestDto>()
            {
                ApiType = ApiType.POST,
                Url = $"{_backChannelUrls.Value.CustomerLoyaltyProgramAPIBaseUri}/AddRewardTransactionForCompleteOrdering",
                Data = requestDto
            });
            return result;
        }

        public int ConvertOrderPriceToRewardPoint(double orderPrice)
        {
            return 100;
        }
    }
}
