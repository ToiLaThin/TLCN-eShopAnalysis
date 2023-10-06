using eShopAnalysis.CouponSaleItemAPI.Dto;
using eShopAnalysis.Dto.BackchannelDto;

namespace eShopAnalysis.CouponSaleItemAPI.Service.BackchannelService
{
    public interface IBackChannelProductCatalogService
    {
        //TODO implement this after determine the productupdateToSaleRequest in BackchannelDto
        Task<BackChannelResponseDto<ProductDto>> UpdateProductToSale();
    }
}
