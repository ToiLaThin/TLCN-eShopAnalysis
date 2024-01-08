using eShopAnalysis.CouponSaleItemAPI.Dto;
using eShopAnalysis.CouponSaleItemAPI.Models;
using eShopAnalysis.CouponSaleItemAPI.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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

        public async Task<ServiceResponseDto<SaleItem>> Get(Guid saleItemId)
        {
            var result = await _uOW.SaleItemRepository.GetAsync(saleItemId);
            return ServiceResponseDto<SaleItem>.Success(result);
        }

        public async Task<ServiceResponseDto<SaleItem>> Update(SaleItem saleItem)
        {
            var transaction = await _uOW.BeginTransactionAsync();
            var result = await _uOW.SaleItemRepository.UpdateAsync(saleItem);
            if (result == null)
            {
                await transaction.RollbackAsync();
                return ServiceResponseDto<SaleItem>.Failure("cannot update this the sale item, rolled back transaction");
            }
            await transaction.CommitAsync();
            return ServiceResponseDto<SaleItem>.Success(result);
        }

        public async Task<ServiceResponseDto<string>> CreateNewInstanceWhenProductModelPriceChanged(Guid oldSaleItemId,
            Guid newSaleItemId,
            Guid oldProductId,
            Guid oldProductModelId,
            Guid newProductId,
            Guid newProductModelId)
        {
            var transaction = await _uOW.BeginTransactionAsync();
            //take the saleItemId old and new
            //find the existing oldSaleItemId, clone it
            var oldSaleItem = await _uOW.SaleItemRepository.GetAsync(oldSaleItemId);
            if (oldSaleItem == null) {
                throw new Exception("Some exception, the saleItemId does not exists any more");
            }
            SaleItem newSaleItem = (SaleItem)oldSaleItem.Clone();

            //change the cloned saleItem Id , date started, then add it to db
            newSaleItem.SaleItemId = newSaleItemId;
            newSaleItem.DateAdded = DateTime.UtcNow;

            //if one of the two guid in saleItem not match the guid in product before cloning new instance, then it's invalid and thí operation should be aborted
            if (newSaleItem.ProductId != oldProductId || newSaleItem.ProductModelId != oldProductModelId) {
                throw new Exception("ProductId or ProductModelId not matched to thóe of product");
            }
            newSaleItem.ProductId =newProductId;
            newSaleItem.ProductModelId = newProductModelId;

            var result = await _uOW.SaleItemRepository.AddAsync(newSaleItem);
            if (result == null) {
                await transaction.RollbackAsync();
                return ServiceResponseDto<string>.Failure("cannot add the new sale item, rolled back transaction");
            }

            //mark the old saleItemId as ended (date and status), this is below to make sure if exception happend, db not changed
            oldSaleItem.SaleItemStatus = Status.Ended;
            oldSaleItem.DateEnded = DateTime.UtcNow;
            result = await _uOW.SaleItemRepository.UpdateAsync(oldSaleItem);
            if (result == null) {
                await transaction.RollbackAsync();
                return ServiceResponseDto<string>.Failure("cannot update the old sale item, rolled back transaction");
            }

            await transaction.CommitAsync();
            return ServiceResponseDto<string>.Success("both add new sale item and update old sale item succeeded");
        }
    }
}
