using eShopAnalysis.ProductCatalogAPI.Application.BackChannelDto;
using eShopAnalysis.ProductCatalogAPI.Application.Result;

namespace eShopAnalysis.ProductCatalogAPI.Application.BackchannelServices
{
    public interface IBackChannelStockInventoryService
    {
        Task<BackChannelResponseDto<StockInventoryDto>> AddNewStockInventory(string productId, string productModelId, string businessKey);
    }
}
