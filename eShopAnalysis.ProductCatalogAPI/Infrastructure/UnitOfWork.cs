using eShopAnalysis.ProductCatalogAPI.Infrastructure.Contract;
using eShopAnalysis.ProductCatalogAPI.Infrastructure.Data;
using MongoDB.Driver;

namespace eShopAnalysis.ProductCatalogAPI.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private MongoDbContext _dbContext;
        private IClientSessionHandle _clientSession;
        private ICatalogRepository _catalogRepository;
        private IProductRepository _productRepository;
        public UnitOfWork(MongoDbContext dbContext)
        {
            _dbContext = dbContext;
            _clientSession = dbContext.GetClientSession();
        }
        public ICatalogRepository CatalogRepository {
            get {
                if (_catalogRepository == null) {
                    _catalogRepository =  new CatalogRepository(_dbContext);
                }
                return _catalogRepository;
            }
        }

        public IProductRepository ProductRepository {
            get {
                if (_productRepository == null) {
                    _productRepository = new ProductRepository(_dbContext);
                }
                return _productRepository;
            }
        }

        public void BeginTransactionAsync() {
            _clientSession.StartTransaction();
        }

        public void CommitTransaction() {
            _clientSession.CommitTransaction();
        }

        public async Task CommitTransactionAsync() {
            await _clientSession.CommitTransactionAsync();
        }
        
        public IClientSessionHandle GetClientSessionHandle() => _clientSession;

        public bool HasActiveTransaction() => _clientSession.IsInTransaction;

        public async Task RollbackTransactionAsync() {
            await _clientSession.AbortTransactionAsync();
        }

        public void RollbackTransaction() {
            _clientSession.AbortTransaction();
        }

        public void Dispose(){
            //have error keep dispose over and over
            //if (_clientSession != null) { 
            //    _clientSession.Dispose(); 
            //}
            //this.Dispose();
            GC.SuppressFinalize(this);
        }

    }
}
