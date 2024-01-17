using eShopAnalysis.Aggregator.Result;
using eShopAnalysis.Aggregator.Services.BackchannelDto;
using eShopAnalysis.Aggregator.Services.BackchannelServices;
using eShopAnalysis.Aggregator.Utilities;
using Microsoft.Extensions.Options;

namespace eShopAnalysis.Aggregator.Services.BackchannelService
{
    public class BackChannelProductCatalogService : IBackChannelProductCatalogService
    {        
        private readonly IServiceProvider _serviceProvider;
        private readonly IOptions<BackChannelCommunication> _backChannelUrls;

        public BackChannelProductCatalogService(IOptions<BackChannelCommunication> backChannelUrls, IServiceProvider serviceProvider)
        {
            _backChannelUrls = backChannelUrls;
            _serviceProvider = serviceProvider;
        }

        public async Task<BackChannelResponseDto<ProductDto>> AddProduct(ProductDto productToAdd)
        {
            var baseService = _serviceProvider.GetRequiredService<IBackChannelBaseService<ProductDto, ProductDto>>();
            var result = await baseService.SendAsync(new BackChannelRequestDto<ProductDto>()
            {
                ApiType = ApiType.POST,
                Url = $"{_backChannelUrls.Value.ProductAPIBaseUri}/AddProduct",
                Data = productToAdd
            });
            return result;
        }

        public async Task<BackChannelResponseDto<IEnumerable<ProductModelInfoResponseDto>>> GetProductModelInfosOfProvider(IEnumerable<ProductModelInfoRequestMetaDto> productModelInfoRequestMetas)
        {
            var baseService = _serviceProvider.GetRequiredService<IBackChannelBaseService<IEnumerable<ProductModelInfoRequestMetaDto>, IEnumerable<ProductModelInfoResponseDto>>>();
            var result = await baseService.SendAsync(new BackChannelRequestDto<IEnumerable<ProductModelInfoRequestMetaDto>>()
            {
                ApiType = ApiType.POST,
                Url = $"{_backChannelUrls.Value.ProductAPIBaseUri}/GetProductModelInfosOfProvider",
                Data = productModelInfoRequestMetas
            });
            return result;
        }

        public async Task<BackChannelResponseDto<ProductDto>> UpdateProductToSaleAsync(Guid productId, Guid productModelId, Guid saleItemId, DiscountType discountType, double discountValue)
        {
            var baseService = _serviceProvider.GetRequiredService<IBackChannelBaseService<ProductUpdateToSaleRequestDto, ProductDto>>();
            var result = await baseService.SendAsync(new BackChannelRequestDto<ProductUpdateToSaleRequestDto>()
            {
                ApiType = ApiType.POST,
                Url = $"{_backChannelUrls.Value.ProductAPIBaseUri}/UpdateProductToOnSale",
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
