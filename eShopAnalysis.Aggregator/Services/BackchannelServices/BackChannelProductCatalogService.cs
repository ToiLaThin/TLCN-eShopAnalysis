using eShopAnalysis.Aggregator.Result;
using eShopAnalysis.Aggregator.Services.BackchannelDto;
using eShopAnalysis.Aggregator.Services.BackchannelServices;
using eShopAnalysis.Aggregator.Utilities;
using Microsoft.Extensions.Options;

namespace eShopAnalysis.Aggregator.Services.BackchannelService
{
    public class BackChannelProductCatalogService : IBackChannelProductCatalogService
    {        
        private readonly IBackChannelBaseService<ProductUpdateToSaleRequestDto, ProductDto> _baseService;
        private readonly IOptions<BackChannelCommunication> _backChannelUrls;

        public BackChannelProductCatalogService(IBackChannelBaseService<ProductUpdateToSaleRequestDto, ProductDto> baseService, IOptions<BackChannelCommunication> backChannelUrls)
        {
            _baseService = baseService;
            _backChannelUrls = backChannelUrls;
        }
        public async Task<BackChannelResponseDto<ProductDto>> UpdateProductToSaleAsync(Guid productId, Guid productModelId, Guid saleItemId, DiscountType discountType, double discountValue)
        {
            var result = await _baseService.SendAsync(new BackChannelRequestDto<ProductUpdateToSaleRequestDto>()
            {
                ApiType = ApiType.POST,
                Url = $"{_backChannelUrls.Value.ProductCatalogAPIBaseUri}/UpdateProductToOnSale",
                Data = new ProductUpdateToSaleRequestDto() { 
                    DiscountType = discountType, 
                    DiscountValue = discountValue, 
                    SaleItemId = saleItemId,
                    ProductId = productId, 
                    ProductModelId = productModelId 
                }
            });
            return result;
            
        }
    }
}
