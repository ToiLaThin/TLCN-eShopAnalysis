using eShopAnalysis.CouponSaleItemAPI.Dto;
using eShopAnalysis.CouponSaleItemAPI.Models;
using eShopAnalysis.CouponSaleItemAPI.Service.BackchannelService;
using eShopAnalysis.CouponSaleItemAPI.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace eShopAnalysis.CouponSaleItemAPI.Service
{
    public class SaleItemService : ISaleItemService
    {
        private IUnitOfWork _uOW;
        private IBackChannelProductCatalogService _backChannelProductCatalogService;
        public SaleItemService(IUnitOfWork uOW, IBackChannelProductCatalogService backChannelProductCatalogService)
        {
            _uOW = uOW;
            _backChannelProductCatalogService = backChannelProductCatalogService;

        }

        public async Task<ServiceResponseDto<SaleItem>> Add(SaleItem saleItem)
        {
            var transaction = await _uOW.BeginTransactionAsync();
            var result = await _uOW.SaleItemRepository.AddAsync(saleItem);
            if (result == null) {
                await transaction.RollbackAsync();
                return ServiceResponseDto<SaleItem>.Failure("cannot add the sale item, rolled back transaction");
            }
            var backChannelResponse = await _backChannelProductCatalogService.UpdateProductToSaleAsync(result.ProductId, result.ProductModelId, result.SaleItemId, result.DiscountType, result.DiscountValue);
            if (backChannelResponse.IsFailed || backChannelResponse.IsException) {
                await transaction.RollbackAsync();
                return ServiceResponseDto<SaleItem>.Failure("cannot update the associate product model, rolled back transaction");
                //TODO also see how you can rollback the change in product, may be sending another request to reset the model
            }
            var x = backChannelResponse.Result; //inspect x to see if it 's updated to isOnSale
            await transaction.CommitAsync();               
            return ServiceResponseDto<SaleItem>.Success(result);
        }

        public async Task<ServiceResponseDto<SaleItem>> Delete(Guid saleItem)
        {
            var transaction = await _uOW.BeginTransactionAsync();
            var result = await _uOW.SaleItemRepository.DeleteAsync(saleItem);
            if (result == null) {
                await transaction.RollbackAsync();
                return ServiceResponseDto<SaleItem>.Failure("cannot delete the sale item, rolled back transaction");
            }
            await transaction.CommitAsync();
            return ServiceResponseDto<SaleItem>.Success(result);
        }

        public async Task<ServiceResponseDto<IEnumerable<SaleItem>>> GetAll()
        {
            var result = await _uOW.SaleItemRepository.GetAsQueryable()
                                                .AsNoTracking()
                                                .ToListAsync();
            return ServiceResponseDto<IEnumerable<SaleItem>>.Success(result);
        }

        public async Task<ServiceResponseDto<SaleItem>> GetCoupon(Guid couponId)
        {
            var result = await _uOW.SaleItemRepository.GetAsync(couponId);
            return ServiceResponseDto<SaleItem>.Success(result);
        }
    }
}
