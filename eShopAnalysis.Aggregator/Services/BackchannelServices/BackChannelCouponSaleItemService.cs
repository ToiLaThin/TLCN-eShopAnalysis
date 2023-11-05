
using eShopAnalysis.Aggregator.Application.BackchannelDto;
using eShopAnalysis.Aggregator.Result;
using eShopAnalysis.Aggregator.Services.BackchannelServices;
using eShopAnalysis.Aggregator.Utilities;
using Microsoft.Extensions.Options;

namespace eShopAnalysis.CartOrderAPI.Application.BackchannelServices
{
    public class BackChannelCouponSaleItemService : IBackChannelCouponSaleItemService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IOptions<BackChannelCommunication> _backChannelUrls;
        public BackChannelCouponSaleItemService(IServiceProvider serviceProvider, IOptions<BackChannelCommunication> backChannleUrls)
        {
            _backChannelUrls = backChannleUrls;
            _serviceProvider = serviceProvider;
        }

        public async Task<BackChannelResponseDto<CouponDto>> RetrieveCouponWithCode(string couponCode)
        {
            var baseService = _serviceProvider.GetRequiredService<IBackChannelBaseService<RetrieveCouponWithCodeRequestDto, CouponDto>>();
            var result = await baseService.SendAsync(new BackChannelRequestDto<RetrieveCouponWithCodeRequestDto>()
            {
                ApiType = ApiType.GET,
                Url = $"{_backChannelUrls.Value.CouponSaleItemAPIBaseUri}/RetrieveCouponWithCode",
                Data = new RetrieveCouponWithCodeRequestDto() { CouponCode = couponCode }
            });
            return result;
        }
    }
}
