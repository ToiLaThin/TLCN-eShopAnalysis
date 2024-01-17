using eShopAnalysis.Aggregator.Models.Dto;
using eShopAnalysis.Aggregator.Result;
using eShopAnalysis.Aggregator.Services.BackchannelDto;
using eShopAnalysis.Aggregator.Services.BackChannelDto;
using eShopAnalysis.Aggregator.Services.BackchannelServices;
using eShopAnalysis.Aggregator.Utilities;
using Microsoft.Extensions.Options;

namespace eShopAnalysis.Aggregator.Services.BackchannelServices
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

        /// <summary>
        /// Get the stock (product model with quantity in inventory) of product model inside the order
        /// </summary>
        /// <param name="productModelIds">product model id inside the order</param>
        /// <returns></returns>
        public async Task<BackChannelResponseDto<IEnumerable<ItemStockResponseDto>>> GetOrderItemsStock(IEnumerable<Guid> productModelIds)
        {
            var baseService = _serviceProvider.GetRequiredService<IBackChannelBaseService<ItemsStockRequestDto, IEnumerable<ItemStockResponseDto>>>();
            var result = await baseService.SendAsync(new BackChannelRequestDto<ItemsStockRequestDto>()
            {
                ApiType = ApiType.POST,
                Url = $"{_backChannelUrls.Value.StockInventoryAPIBaseUri}/GetStockOfModels",
                Data = new ItemsStockRequestDto(productModelIds)
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

        public async Task<BackChannelResponseDto<IEnumerable<StockInventoryDto>>> AddNewStockInventories(IEnumerable<StockInventoryDto> stocksToAdd)
        {
            var baseService = _serviceProvider.GetRequiredService<IBackChannelBaseService<IEnumerable<StockInventoryDto>, IEnumerable<StockInventoryDto>>>();
            var result = await baseService.SendAsync(new BackChannelRequestDto<IEnumerable<StockInventoryDto>>()
            {
                ApiType = ApiType.POST,
                Url = $"{_backChannelUrls.Value.StockInventoryAPIBaseUri}/AddNewStockInventories",
                Data = stocksToAdd
            });
            return result;
        }
    }
}
