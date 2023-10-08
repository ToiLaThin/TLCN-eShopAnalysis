using eShopAnalysis.CartOrderAPI.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace eShopAnalysis.CartOrderAPI.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        public ICartRepository CartRepository { get; }
        IDbContextTransaction GetCurrentTransaction();

        bool HasActiveTransaction();

        Task<IDbContextTransaction> BeginTransactionAsync();
        public Task CommitTransactionAsync(IDbContextTransaction transaction);

        public void RollbackTransaction();

        void Dispose();
    }
}
