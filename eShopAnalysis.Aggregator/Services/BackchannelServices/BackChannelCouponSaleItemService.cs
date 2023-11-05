
using eShopAnalysis.Aggregator.Dto;
using eShopAnalysis.Aggregator.Result;
using eShopAnalysis.Aggregator.Services.BackchannelDto;
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

        public async Task<BackChannelResponseDto<SaleItemDto>> AddSaleItem(SaleItem saleItem)
        {
            var baseService = _serviceProvider.GetRequiredService<IBackChannelBaseService<SaleItem, SaleItemDto>>();
            var result = await baseService.SendAsync(new BackChannelRequestDto<SaleItem>()
            {
                ApiType = ApiType.POST,
                Url = $"{_backChannelUrls.Value.SaleItemAPIBaseUri}/AddSaleItem",
                Data = saleItem
            });
            return result;
        }

        public async Task<BackChannelResponseDto<CouponDto>> RetrieveCouponWithCode(string couponCode)
        {
            var baseService = _serviceProvider.GetRequiredService<IBackChannelBaseService<RetrieveCouponWithCodeRequestDto, CouponDto>>();
            var result = await baseService.SendAsync(new BackChannelRequestDto<RetrieveCouponWithCodeRequestDto>()
            {
                ApiType = ApiType.GET,
                Url = $"{_backChannelUrls.Value.CouponAPIBaseUri}/RetrieveCouponWithCode",
                Data = new RetrieveCouponWithCodeRequestDto() { CouponCode = couponCode }
            });
            return result;
        }
    }
}
