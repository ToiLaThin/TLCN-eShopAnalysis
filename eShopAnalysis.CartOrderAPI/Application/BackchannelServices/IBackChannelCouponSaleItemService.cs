using eShopAnalysis.CartOrderAPI.Application.BackchannelDto;
using eShopAnalysis.CartOrderAPI.Application.Result;

namespace eShopAnalysis.CartOrderAPI.Application.BackchannelServices
{
    public interface IBackChannelCouponSaleItemService
    {
        Task<BackChannelResponseDto<CouponDto>> RetrieveCouponWithCode(string couponCode);
    }
}