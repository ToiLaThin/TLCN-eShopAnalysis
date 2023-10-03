using eShopAnalysis.ProductCatalogAPI.Application.BackChannelDto;
using eShopAnalysis.ProductCatalogAPI.Application.Result;
using eShopAnalysis.ProductCatalogAPI.Utilities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;

namespace eShopAnalysis.ProductCatalogAPI.Application.BackchannelServices
{
    public class BackChannelStockInventoryService : IBackChannelStockInventoryService
    {
        private readonly IBackChannelBaseService<StockInventoryDto, StockInventoryDto> _baseService;
        private readonly IOptions<BackChannelCommunication> _backChannelUrls;
        public BackChannelStockInventoryService(IBackChannelBaseService<StockInventoryDto, StockInventoryDto> baseService, IOptions<BackChannelCommunication> backChannelUrls)
        {
            _baseService = baseService;
            _backChannelUrls = backChannelUrls;
        }
        public async Task<BackChannelResponseDto<StockInventoryDto>> AddNewStockInventory(string productId, string productModelId, string businessKey)
        {
            var result = await _baseService.SendAsync(new BackChannelRequestDto<StockInventoryDto>()
            {
                ApiType = ApiType.POST,
                Url = $"{_backChannelUrls.Value.StockInventoryAPIBaseUri}/AddStock",
                Data = new StockInventoryDto() { ProductId = productId, ProductModelId = productModelId, ProductBusinessKey = businessKey }
            });
            return result;
        }
    }
}
