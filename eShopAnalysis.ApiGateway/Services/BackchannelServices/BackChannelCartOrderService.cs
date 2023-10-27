using eShopAnalysis.ApiGateway.Result;
using eShopAnalysis.ApiGateway.Services.BackchannelDto;
using eShopAnalysis.ProductCatalogAPI.Utilities;
using Microsoft.Extensions.Options;

namespace eShopAnalysis.ApiGateway.Services.BackchannelServices
{
    public class BackChannelCartOrderService : IBackChannelCartOrderService
    {
        private readonly IBackChannelBaseService<PagingOrderRequestDto, OrderItemsResponseDto> _baseService;
        private readonly IOptions<BackChannelCommunication> _backChannelUrls;
        public BackChannelCartOrderService(IBackChannelBaseService<PagingOrderRequestDto, OrderItemsResponseDto> baseService, IOptions<BackChannelCommunication> backChannelUrls)
        {
            _baseService = baseService;
            _backChannelUrls = backChannelUrls;
        }

        public async Task<BackChannelResponseDto<OrderItemsResponseDto>> GetToApprovedOrders(int limit = 15)
        {
            var result = await _baseService.SendAsync(new BackChannelRequestDto<PagingOrderRequestDto>()
            {
                ApiType = ApiType.GET,
                Url = $"{_backChannelUrls.Value.StockInventoryAPIBaseUri}/GetToApprovedOrders",
                Data = new PagingOrderRequestDto() { Limit = limit }
            });
            return result;
        }
    }
}
