using eShopAnalysis.Aggregator.Result;
using eShopAnalysis.Aggregator.Services.BackchannelDto;
using eShopAnalysis.Aggregator.Utilities;
using Microsoft.Extensions.Options;

namespace eShopAnalysis.Aggregator.Services.BackchannelServices
{
    public class BackChannelCartOrderService : IBackChannelCartOrderService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IOptions<BackChannelCommunication> _backChannelUrls;
        public BackChannelCartOrderService(
            IServiceProvider serviceProvider,
            IOptions<BackChannelCommunication> backChannelUrls
            )
        {
            _serviceProvider = serviceProvider;
            _backChannelUrls = backChannelUrls;
        }

        public async Task<BackChannelResponseDto<IEnumerable<OrderItemsResponseDto>>> GetToApprovedOrders(int limit = 15)
        {
            var baseService = _serviceProvider.GetRequiredService<IBackChannelBaseService<PagingOrderRequestDto, IEnumerable<OrderItemsResponseDto>>>();
            var result = await baseService.SendAsync(new BackChannelRequestDto<PagingOrderRequestDto>()
            {
                ApiType = ApiType.POST,
                Url = $"{_backChannelUrls.Value.OrderAPIBaseUri}/GetToApprovedOrders",
                Data = new PagingOrderRequestDto(limit)
            });
            return result;
        }

        public async Task<BackChannelResponseDto<IEnumerable<Guid>>> BulkApproveOrder(IEnumerable<Guid> orderIdsToApprove)
        {
            var baseService = _serviceProvider.GetRequiredService<IBackChannelBaseService<IEnumerable<Guid>, IEnumerable<Guid>>>();
            var result = await baseService.SendAsync(new BackChannelRequestDto<IEnumerable<Guid>>()
            {
                ApiType = ApiType.PUT,
                Url = $"{_backChannelUrls.Value.OrderAPIBaseUri}/BulkApproveOrder",
                Data = orderIdsToApprove
            });
            return result;
        }

        public async Task<BackChannelResponseDto<object>> AddCart(CartConfirmRequestToCartApiDto requestToCartApiDto)
        {
            var baseService = _serviceProvider.GetRequiredService<IBackChannelBaseService<CartConfirmRequestToCartApiDto, object>>();
            var result = await baseService.SendAsync(new BackChannelRequestDto<CartConfirmRequestToCartApiDto>()
            {
                ApiType = ApiType.POST,
                Url = $"{_backChannelUrls.Value.CartAPIBaseUri}/AddCart",
                Data = requestToCartApiDto
            });
            return result;
        }
    }
}
