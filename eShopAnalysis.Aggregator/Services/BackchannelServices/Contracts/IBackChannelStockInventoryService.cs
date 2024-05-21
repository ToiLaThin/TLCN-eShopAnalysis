using eShopAnalysis.Aggregator.Result;
using eShopAnalysis.Aggregator.Services.BackchannelDto;

namespace eShopAnalysis.Aggregator.Services.BackchannelServices
{
    public interface IBackChannelStockInventoryService
    {
        Task<BackChannelResponseDto<IEnumerable<ItemStockResponseDto>>> GetOrderItemsStock(IEnumerable<Guid> productModelIds);

        Task<BackChannelResponseDto<IEnumerable<ItemStockResponseDto>>> GetAllItemsStock();

        Task<BackChannelResponseDto<IEnumerable<ItemStockResponseDto>>> DecreaseStockItems(IEnumerable<StockDecreaseRequestDto> stockDecreaseReqs);

        Task<BackChannelResponseDto<IEnumerable<ItemStockResponseDto>>> IncreaseStockItems(IEnumerable<StockIncreaseRequestDto> stockIncreaseReqs);

        Task<BackChannelResponseDto<IEnumerable<StockInventoryDto>>> AddNewStockInventories(IEnumerable<StockInventoryDto> stocksToAdd);
    }
}
