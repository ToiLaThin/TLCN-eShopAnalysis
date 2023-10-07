using eShopAnalysis.CouponSaleItemAPI.Dto;
using eShopAnalysis.CouponSaleItemAPI.Dto.BackchannelDto;
using eShopAnalysis.CouponSaleItemAPI.Models;
using eShopAnalysis.CouponSaleItemAPI.Service.BackChannelService;
using eShopAnalysis.CouponSaleItemAPI.Utilities;
using eShopAnalysis.Dto.BackchannelDto;
using Microsoft.Extensions.Options;

namespace eShopAnalysis.CouponSaleItemAPI.Service.BackchannelService
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
        public async Task<BackChannelResponseDto<ProductDto>> UpdateProductToSaleAsync(Guid productId, Guid productModelId, DiscountType discountType, double discountValue)
        {
            var result = await _baseService.SendAsync(new BackChannelRequestDto<ProductUpdateToSaleRequestDto>()
            {
                ApiType = ApiType.POST,
                Url = $"{_backChannelUrls.Value.ProductCatalogAPIBaseUri}/UpdateProductToOnSale",
                Data = new ProductUpdateToSaleRequestDto() { 
                    DiscountType = discountType, 
                    DiscountValue = discountValue, 
                    ProductId = productId, 
                    ProductModelId = productModelId 
                }
            });
            return result;
            
        }
    }
}
