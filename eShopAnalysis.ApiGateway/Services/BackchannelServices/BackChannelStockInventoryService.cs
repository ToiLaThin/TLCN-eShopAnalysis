using eShopAnalysis.ApiGateway.Result;
using eShopAnalysis.ApiGateway.Services.BackchannelDto;
using eShopAnalysis.ProductCatalogAPI.Utilities;
using Microsoft.Extensions.Options;

namespace eShopAnalysis.ApiGateway.Services.BackchannelServices
{
    public class BackChannelStockInventoryService : IBackChannelStockInventoryService
    {
        private readonly IBackChannelBaseService<IEnumerable<Guid>, IEnumerable<ItemStockResponseDto>> _baseService;
        private readonly IOptions<BackChannelCommunication> _backChannelUrls;
        public BackChannelStockInventoryService(IBackChannelBaseService<IEnumerable<Guid>, IEnumerable<ItemStockResponseDto>> baseService, IOptions<BackChannelCommunication> backChannelUrls)
        {
            _baseService = baseService;
            _backChannelUrls = backChannelUrls;
        }
        public async Task<BackChannelResponseDto<IEnumerable<ItemStockResponseDto>>> GetOrderItemsStock(IEnumerable<Guid> productModelIds)
        {
            var result = await _baseService.SendAsync(new BackChannelRequestDto<IEnumerable<Guid>>()
            {
                ApiType = ApiType.GET,
                Url = $"{_backChannelUrls.Value.StockInventoryAPIBaseUri}/GetOrderItemsStock",
                Data = productModelIds
            });
            return result;
        }
    }
}
