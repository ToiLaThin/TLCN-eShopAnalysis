using MongoDB.Driver;

namespace eShopAnalysis.ProductCatalogAPI.Infrastructure.Contract
{
    public interface IUnitOfWork: IDisposable
    {
        public ICatalogRepository CatalogRepository { get; }

        public IProductRepository ProductRepository { get; }
        IClientSessionHandle GetClientSessionHandle();

        public bool HasActiveTransaction();

        public void BeginTransactionAsync();

        public void CommitTransaction();

        public Task CommitTransactionAsync();

        public void RollbackTransaction();

        public Task RollbackTransactionAsync();

        void Dispose();
    }
}
