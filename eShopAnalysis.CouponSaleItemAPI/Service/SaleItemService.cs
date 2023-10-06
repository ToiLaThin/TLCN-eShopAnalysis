using eShopAnalysis.CouponSaleItemAPI.Models;
using eShopAnalysis.CouponSaleItemAPI.UnitOfWork;

namespace eShopAnalysis.CouponSaleItemAPI.Service
{
    public class SaleItemService : ISaleItemService
    {
        private IUnitOfWork _uOW;
        public SaleItemService(IUnitOfWork uOW)
        {
            _uOW = uOW;
        }

        public async Task<SaleItem> Add(SaleItem saleItem)
        {
            var transaction = await _uOW.BeginTransactionAsync();
            var result = _uOW.SaleItemRepository.Add(saleItem);
            if (result == null)
            {
                transaction.RollbackAsync();
                return null;
            }
            else
            {
                transaction.CommitAsync();
                return result;
            }
        }

        public async Task<SaleItem> Delete(Guid saleItem)
        {
            var transaction = await _uOW.BeginTransactionAsync();
            var result = _uOW.SaleItemRepository.Delete(saleItem);
            if (result == null)
            {
                transaction.RollbackAsync();
                return null;
            }
            else
            {
                transaction.CommitAsync();
                return result;
            }
        }

        public IEnumerable<SaleItem> GetAll()
        {
            var result = _uOW.SaleItemRepository.GetAll();
            return result;
        }

        public SaleItem GetCoupon(Guid couponId)
        {
            var result = _uOW.SaleItemRepository.Get(couponId);
            return result;
        }
    }
}
