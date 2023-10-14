using eShopAnalysis.CouponSaleItemAPI.Dto;
using eShopAnalysis.CouponSaleItemAPI.Models;
using eShopAnalysis.Dto.BackchannelDto;

namespace eShopAnalysis.CouponSaleItemAPI.Service.BackchannelService
{
    public interface IBackChannelProductCatalogService
    {
        Task<BackChannelResponseDto<ProductDto>> UpdateProductToSaleAsync(Guid productId, Guid productModelId, Guid saleItemId, DiscountType discountType, double discountValue);
    }
}
