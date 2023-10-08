using eShopAnalysis.CartOrderAPI.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace eShopAnalysis.CartOrderAPI.Infrastructure
{

    public class UnitOfWork : IUnitOfWork
    {
        private ICartRepository _cartRepository;
        private readonly OrderCartContext _orderCartContext;
        private IDbContextTransaction _currentTransaction;
        public ICartRepository CartRepository
        {
            get
            {
                if (_cartRepository == null)
                {
                    _cartRepository = new CartRepository(this._orderCartContext);
                }
                return _cartRepository;
            }
        }
        public UnitOfWork(OrderCartContext orderCartContext)
        {
            _orderCartContext = orderCartContext;
        }
        public void Dispose()
        {
            _orderCartContext.Dispose();
            GC.SuppressFinalize(this);
        }

        public bool HasActiveTransaction() => _currentTransaction != null;

        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null) return null;
            _currentTransaction = await _orderCartContext.Database.BeginTransactionAsync();
            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) return;
            try
            {
                await this._orderCartContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
    }
}
