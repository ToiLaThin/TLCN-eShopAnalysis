using eShopAnalysis.CouponSaleItemAPI.Repository;
using Microsoft.EntityFrameworkCore.Storage;

namespace eShopAnalysis.CouponSaleItemAPI.UnitOfWork
{
    public interface IUnitOfWork: IDisposable
    {
        public ICouponRepository CouponRepository { get; }
        public ISaleItemRepository SaleItemRepository { get; }
        IDbContextTransaction GetCurrentTransaction();

        bool HasActiveTransaction();

        Task<IDbContextTransaction> BeginTransactionAsync();
        public Task CommitTransactionAsync(IDbContextTransaction transaction);

        public void RollbackTransaction();

        void Dispose();
    }
}
