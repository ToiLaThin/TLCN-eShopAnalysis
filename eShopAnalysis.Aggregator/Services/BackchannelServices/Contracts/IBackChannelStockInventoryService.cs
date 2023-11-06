using eShopAnalysis.Aggregator.Models.Dto;
using eShopAnalysis.Aggregator.Result;
using eShopAnalysis.Aggregator.Services.BackchannelDto;
using eShopAnalysis.Aggregator.Services.BackChannelDto;

namespace eShopAnalysis.Aggregator.Services.BackchannelServices
{
    public interface IBackChannelStockInventoryService
    {
        Task<BackChannelResponseDto<IEnumerable<ItemStockResponseDto>>> GetOrderItemsStock(IEnumerable<Guid> productModelIds);

        Task<BackChannelResponseDto<IEnumerable<ItemStockResponseDto>>> DecreaseStockItems(IEnumerable<StockDecreaseRequestDto> stockDecreaseReqs);

        Task<BackChannelResponseDto<IEnumerable<StockInventoryDto>>> AddNewStockInventories(IEnumerable<StockInventoryDto> stocksToAdd);
    }
}
