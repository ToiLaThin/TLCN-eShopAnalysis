using eShopAnalysis.PaymentAPI.Repository;
using Microsoft.EntityFrameworkCore.Storage;

namespace eShopAnalysis.PaymentAPI.UnitOfWork
{
    public interface IUnitOfWork: IDisposable
    {
        public IUserCustomerMappingRepository UserCustomerMappingRepository { get; }
        IDbContextTransaction GetCurrentTransaction();

        bool HasActiveTransaction();

        Task<IDbContextTransaction> BeginTransactionAsync();

        public Task CommitTransactionAsync(IDbContextTransaction transaction);

        public void RollbackTransaction();

        void Dispose();
    }
}
