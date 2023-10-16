
using eShopAnalysis.CartOrderAPI.Application.BackchannelDto;
using eShopAnalysis.CartOrderAPI.Application.Result;
using eShopAnalysis.CartOrderAPI.Utilities;
using Microsoft.Extensions.Options;

namespace eShopAnalysis.CartOrderAPI.Application.BackchannelServices
{
    public class BackChannelCouponSaleItemService : IBackChannelCouponSaleItemService
    {
        private readonly IBackChannelBaseService<RetrieveCouponWithCodeRequestDto, CouponDto> _baseService;
        private readonly IOptions<BackChannelCommunication> _backChannelUrls;
        public BackChannelCouponSaleItemService(IBackChannelBaseService<RetrieveCouponWithCodeRequestDto, CouponDto> baseService, IOptions<BackChannelCommunication> backChannleUrls)
        {
            _baseService = baseService;
            _backChannelUrls = backChannleUrls;
        }

        public async Task<BackChannelResponseDto<CouponDto>> RetrieveCouponWithCode(string couponCode)
        {
            var result = await _baseService.SendAsync(new BackChannelRequestDto<RetrieveCouponWithCodeRequestDto>()
            {
                ApiType = ApiType.GET,
                Url = $"{_backChannelUrls.Value.CouponSaleItemAPIBaseUri}/RetrieveCouponWithCode",
                Data = new RetrieveCouponWithCodeRequestDto() { CouponCode = couponCode }
            });
            return result;
        }
    }
}
