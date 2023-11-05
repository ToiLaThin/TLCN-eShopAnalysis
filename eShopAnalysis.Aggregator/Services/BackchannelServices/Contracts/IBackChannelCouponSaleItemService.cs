using eShopAnalysis.Aggregator.Application.BackchannelDto;
using eShopAnalysis.Aggregator.Result;

namespace eShopAnalysis.Aggregator.Services.BackchannelServices
{
    public interface IBackChannelCouponSaleItemService
    {
        Task<BackChannelResponseDto<CouponDto>> RetrieveCouponWithCode(string couponCode);
    }
}