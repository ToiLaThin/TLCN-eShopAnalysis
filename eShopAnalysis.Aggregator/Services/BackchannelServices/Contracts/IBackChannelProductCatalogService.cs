using eShopAnalysis.Aggregator.Result;
using eShopAnalysis.Aggregator.Services.BackchannelDto;

namespace eShopAnalysis.Aggregator.Services.BackchannelServices
{
    public interface IBackChannelProductCatalogService
    {
        Task<BackChannelResponseDto<ProductDto>> UpdateProductToSaleAsync(Guid productId, Guid productModelId, Guid saleItemId, DiscountType discountType, double discountValue);

        Task<BackChannelResponseDto<ProductDto>> AddProduct(ProductDto productToAdd);

        Task<BackChannelResponseDto<IEnumerable<ProductModelInfoResponseDto>>> GetProductModelInfosOfProvider(IEnumerable<ProductModelInfoRequestMetaDto> productModelInfoRequestMetas);
    }
}
