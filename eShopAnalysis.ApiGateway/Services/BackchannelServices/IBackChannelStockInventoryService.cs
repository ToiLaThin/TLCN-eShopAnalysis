
using eShopAnalysis.ApiGateway.Result;
using eShopAnalysis.ApiGateway.Services.BackchannelDto;

namespace eShopAnalysis.ApiGateway.Services.BackchannelServices
{
    public interface IBackChannelStockInventoryService
    {
        Task<BackChannelResponseDto<IEnumerable<ItemStockResponseDto>>> GetOrderItemsStock(IEnumerable<Guid> productModelIds);
    }
}
