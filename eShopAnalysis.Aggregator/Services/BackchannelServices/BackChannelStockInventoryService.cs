using eShopAnalysis.Aggregator.Models.Dto;
using eShopAnalysis.Aggregator.Result;
using eShopAnalysis.Aggregator.Services.BackchannelDto;
using eShopAnalysis.Aggregator.Services.BackchannelServices;
using eShopAnalysis.Aggregator.Utilities;
using Microsoft.Extensions.Options;

namespace eShopAnalysis.ApiGateway.Services.BackchannelServices
{
    public class BackChannelStockInventoryService : IBackChannelStockInventoryService
    {
        //(done using service provider)
        //TODO might need refactor: make backchannelStockInventoryService generic so that it does not have to inject to much
        //but this will make controller inject more service with different generic argument or change the service to make it use reflection
        private readonly IServiceProvider _serviceProvider;
        private readonly IOptions<BackChannelCommunication> _backChannelUrls;
        public BackChannelStockInventoryService(
            IOptions<BackChannelCommunication> backChannelUrls,
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _backChannelUrls = backChannelUrls;
        }
        public async Task<BackChannelResponseDto<IEnumerable<ItemStockResponseDto>>> GetOrderItemsStock(IEnumerable<Guid> productModelIds)
        {
            var baseService = _serviceProvider.GetRequiredService<IBackChannelBaseService<OrderItemsStockRequestDto, IEnumerable<ItemStockResponseDto>>>();
            var result = await baseService.SendAsync(new BackChannelRequestDto<OrderItemsStockRequestDto>()
            {
                ApiType = ApiType.POST,
                Url = $"{_backChannelUrls.Value.StockInventoryAPIBaseUri}/GetStockOfModels",
                Data = new OrderItemsStockRequestDto(productModelIds)
            });
            return result;
        }

        public async Task<BackChannelResponseDto<IEnumerable<ItemStockResponseDto>>> DecreaseStockItems(IEnumerable<StockDecreaseRequestDto> stockDecreaseReqs)
        {
            var baseService = _serviceProvider.GetRequiredService<IBackChannelBaseService<IEnumerable<StockDecreaseRequestDto>, IEnumerable<ItemStockResponseDto>>>();
            var result = await baseService.SendAsync(new BackChannelRequestDto<IEnumerable<StockDecreaseRequestDto>>()
            {
                ApiType = ApiType.POST,
                Url = $"{_backChannelUrls.Value.StockInventoryAPIBaseUri}/DecreaseStockItems",
                Data = stockDecreaseReqs
            });
            return result;
        }
    }
}
