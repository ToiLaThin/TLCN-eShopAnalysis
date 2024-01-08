using eShopAnalysis.CouponSaleItemAPI.Dto;
using eShopAnalysis.CouponSaleItemAPI.Models;

namespace eShopAnalysis.CouponSaleItemAPI.Service
{
    public interface ISaleItemService
    {
        Task<ServiceResponseDto<SaleItem>> Add(SaleItem saleItem);
        Task<ServiceResponseDto<SaleItem>> Delete(Guid saleItem);

        Task<ServiceResponseDto<SaleItem>> Update(SaleItem saleItem);

        Task<ServiceResponseDto<IEnumerable<SaleItem>>> GetAll();
        Task<ServiceResponseDto<SaleItem>> Get(Guid saleItemId);

        Task<ServiceResponseDto<string>> CreateNewInstanceWhenProductModelPriceChanged(
            Guid oldSaleItemId,
            Guid newSaleItemId,
            Guid oldProductId,
            Guid oldProductModelId,
            Guid newProductId,
            Guid newProductModelId
            );
    }
}