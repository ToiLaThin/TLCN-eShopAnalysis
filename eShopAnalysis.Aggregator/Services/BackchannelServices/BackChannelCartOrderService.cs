using eShopAnalysis.Aggregator.Result;
using eShopAnalysis.Aggregator.Services.BackchannelDto;
using eShopAnalysis.Aggregator.Services.BackchannelDto.CartOrder;
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

        public async Task<BackChannelResponseDto<CartSummaryResponseDto>> AddCart(CartConfirmRequestToCartApiDto requestToCartApiDto)
        {
            var baseService = _serviceProvider.GetRequiredService<IBackChannelBaseService<CartConfirmRequestToCartApiDto, CartSummaryResponseDto>>();
            var result = await baseService.SendAsync(new BackChannelRequestDto<CartConfirmRequestToCartApiDto>()
            {
                ApiType = ApiType.POST,
                Url = $"{_backChannelUrls.Value.CartAPIBaseUri}/AddCart",
                Data = requestToCartApiDto
            });
            return result;
        }

        public async Task<BackChannelResponseDto<OrderAggregateCartResponseDto>> GetOrderAggregateCartByCartId(Guid cartId)
        {
            var baseService = _serviceProvider.GetRequiredService<IBackChannelBaseService<ReferenceTypeWrapperDto<Guid>, OrderAggregateCartResponseDto>>();
            var result = await baseService.SendAsync(new BackChannelRequestDto<ReferenceTypeWrapperDto<Guid>>
            {
                ApiType = ApiType.POST,
                Url = $"{_backChannelUrls.Value.OrderAPIBaseUri}/GetOrderAggregateCartByCartId",
                Data = new ReferenceTypeWrapperDto<Guid>(cartId)
            });
            return result;
        }
    }
}
