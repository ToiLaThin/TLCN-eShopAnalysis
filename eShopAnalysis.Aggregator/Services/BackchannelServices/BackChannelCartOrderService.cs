using eShopAnalysis.Aggregator.Result;
using eShopAnalysis.Aggregator.Services.BackchannelDto;
using eShopAnalysis.Aggregator.Services.BackchannelServices;
using eShopAnalysis.Aggregator.Utilities;
using Microsoft.Extensions.Options;

namespace eShopAnalysis.ApiGateway.Services.BackchannelServices
{
    public class BackChannelCartOrderService : IBackChannelCartOrderService
    {
        private readonly IBackChannelBaseService<PagingOrderRequestDto, IEnumerable<OrderItemsResponseDto>> _baseService;
        private readonly IOptions<BackChannelCommunication> _backChannelUrls;
        public BackChannelCartOrderService(IBackChannelBaseService<PagingOrderRequestDto, IEnumerable<OrderItemsResponseDto>> baseService, IOptions<BackChannelCommunication> backChannelUrls)
        {
            _baseService = baseService;
            _backChannelUrls = backChannelUrls;
        }

        public async Task<BackChannelResponseDto<IEnumerable<OrderItemsResponseDto>>> GetToApprovedOrders(int limit = 15)
        {
            var result = await _baseService.SendAsync(new BackChannelRequestDto<PagingOrderRequestDto>()
            {
                ApiType = ApiType.POST,
                Url = $"{_backChannelUrls.Value.OrderAPIBaseUri}/GetToApprovedOrders",
                Data = new PagingOrderRequestDto(limit)
            });
            return result;
        }
    }
}
