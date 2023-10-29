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
        private readonly IBackChannelBaseService<OrderItemsStockRequestDto, IEnumerable<ItemStockResponseDto>> _baseService;
        //TODO might need refactor: make backchannelStockInventoryService generic so that it does not have to inject to much
        //but this will make controller inject more service with different generic argument or change the service to make it use reflection
        private readonly IBackChannelBaseService<IEnumerable<StockDecreaseRequestDto>, IEnumerable<ItemStockResponseDto>> _baseStockDecreaseService;
        private readonly IOptions<BackChannelCommunication> _backChannelUrls;
        public BackChannelStockInventoryService(
            IBackChannelBaseService<OrderItemsStockRequestDto, IEnumerable<ItemStockResponseDto>> baseService, 
            IOptions<BackChannelCommunication> backChannelUrls,
            IBackChannelBaseService<IEnumerable<StockDecreaseRequestDto>, IEnumerable<ItemStockResponseDto>> baseStockDecreaseService)
        {
            _baseService = baseService;
            _backChannelUrls = backChannelUrls;
            _baseStockDecreaseService = baseStockDecreaseService;
        }
        public async Task<BackChannelResponseDto<IEnumerable<ItemStockResponseDto>>> GetOrderItemsStock(IEnumerable<Guid> productModelIds)
        {
            var result = await _baseService.SendAsync(new BackChannelRequestDto<OrderItemsStockRequestDto>()
            {
                ApiType = ApiType.POST,
                Url = $"{_backChannelUrls.Value.StockInventoryAPIBaseUri}/GetStockOfModels",
                Data = new OrderItemsStockRequestDto(productModelIds)
            });
            return result;
        }

        public async Task<BackChannelResponseDto<IEnumerable<ItemStockResponseDto>>> DecreaseStockItems(IEnumerable<StockDecreaseRequestDto> stockDecreaseReqs)
        {
            var result = await _baseStockDecreaseService.SendAsync(new BackChannelRequestDto<IEnumerable<StockDecreaseRequestDto>>()
            {
                ApiType = ApiType.POST,
                Url = $"{_backChannelUrls.Value.StockInventoryAPIBaseUri}/DecreaseStockItems",
                Data = stockDecreaseReqs
            });
            return result;
        }
    }
}
