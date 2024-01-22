using eShopAnalysis.Aggregator.Result;
using eShopAnalysis.Aggregator.Services.BackchannelDto;

namespace eShopAnalysis.Aggregator.Services.BackchannelServices
{
    public interface IBackChannelCouponSaleItemService
    {
        Task<BackChannelResponseDto<CouponDto>> RetrieveCouponWithCode(string couponCode);

        Task<BackChannelResponseDto<SaleItemDto>> AddSaleItem(Aggregator.Services.BackchannelDto.SaleItemDto saleItem);
    }
}