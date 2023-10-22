using eShopAnalysis.PaymentAPI.Data;
using eShopAnalysis.PaymentAPI.Repository;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace eShopAnalysis.PaymentAPI.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private PaymentContext _dbContext;
        private IDbContextTransaction _currentTransaction;
        private IUserCustomerMappingRepository _userCustomerMappingRepository;
        private IStripePaymentTransactionRepository _stripePaymentTransactionRepository;
        private IMomoPaymentTransactionRepository _momoPaymentTransactionRepository;
        public IUserCustomerMappingRepository UserCustomerMappingRepository {
            get
            {
                if (this._userCustomerMappingRepository == null) {
                    this._userCustomerMappingRepository = new UserCustomerMappingRepository(_dbContext);
                }
                return this._userCustomerMappingRepository;
            }
        }

        public IStripePaymentTransactionRepository StripePaymentTransactionRepository
        {
            get {
                if (_stripePaymentTransactionRepository == null) {
                    _stripePaymentTransactionRepository = new StripePaymentTransactionRepository(this._dbContext);
                }
                return _stripePaymentTransactionRepository;
            }
        }

        public IMomoPaymentTransactionRepository MomoPaymentTransactionRepository
        {
            get {
                if (_momoPaymentTransactionRepository == null)
                {
                    _momoPaymentTransactionRepository = new MomoPaymentTransactionRepository(this._dbContext);
                }
                return _momoPaymentTransactionRepository;
            }
        }

        public UnitOfWork(PaymentContext dbContext) { _dbContext = dbContext; }

        public void Dispose()
        {
            _dbContext.Dispose();
            GC.SuppressFinalize(this);
        }

        public bool HasActiveTransaction() => _currentTransaction != null;

        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null) return null;
            _currentTransaction = await _dbContext.Database.BeginTransactionAsync();
            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) return;
            try
            {
                await this._dbContext.SaveChangesAsync();
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
