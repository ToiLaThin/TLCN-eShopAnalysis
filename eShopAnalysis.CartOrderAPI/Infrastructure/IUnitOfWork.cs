using eShopAnalysis.CartOrderAPI.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace eShopAnalysis.CartOrderAPI.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        public ICartRepository CartRepository { get; }

        public IOrderRepository OrderRepository { get; }
        IDbContextTransaction GetCurrentTransaction();

        bool HasActiveTransaction();

        Task<IDbContextTransaction> BeginTransactionAsync();

        //will also commit all domain event and await before commit
        public Task CommitTransactionAsync(IDbContextTransaction transaction);

        public void RollbackTransaction();

        void Dispose();
    }
}
