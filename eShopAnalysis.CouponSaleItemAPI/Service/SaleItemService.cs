using eShopAnalysis.CouponSaleItemAPI.Dto;
using eShopAnalysis.CouponSaleItemAPI.Models;
using eShopAnalysis.CouponSaleItemAPI.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace eShopAnalysis.CouponSaleItemAPI.Service
{
    public class SaleItemService : ISaleItemService
    {
        private IUnitOfWork _uOW;
        public SaleItemService(IUnitOfWork uOW)
        {
            _uOW = uOW;
        }

        public async Task<ServiceResponseDto<SaleItem>> Add(SaleItem saleItem)
        {
            var transaction = await _uOW.BeginTransactionAsync();
            var result = await _uOW.SaleItemRepository.AddAsync(saleItem);
            if (result == null) {
                await transaction.RollbackAsync();
                return ServiceResponseDto<SaleItem>.Failure("cannot add the sale item, rolled back transaction");
            }
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
