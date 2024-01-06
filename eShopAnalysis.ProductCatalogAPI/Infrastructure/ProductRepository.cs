using eShopAnalysis.ProductCatalogAPI.Domain.Models;
using eShopAnalysis.ProductCatalogAPI.Domain.Models.Aggregator;
using eShopAnalysis.ProductCatalogAPI.Infrastructure.Data;
using MongoDB.Driver;

namespace eShopAnalysis.ProductCatalogAPI.Infrastructure
{
    public class ProductRepository : IProductRepository
    {
        private readonly MongoDbContext _context;

        public ProductRepository(MongoDbContext context) { 
            _context = context;
        }

        #region Sync Methods
        public Product Get(Guid id)
        {
            Product result = _context.ProductCollection.AsQueryable().Where(p => p.ProductId== id).FirstOrDefault();
            return result;
        }        

        public void Add(Product product, IClientSessionHandle sessionHandle = null)
        {
            if (sessionHandle == null) {
                _context.ProductCollection.InsertOne(product);
                return;
            }
            if (!sessionHandle.IsInTransaction) throw new InvalidOperationException("used not correctly");
            _context.ProductCollection.InsertOne(session: sessionHandle, product);
        }

        public bool Replace(Product newProduct, IClientSessionHandle sessionHandle = null)
        {
            bool sessionIsNull = sessionHandle == null;
            if (!sessionIsNull && !sessionHandle.IsInTransaction) 
                throw new InvalidOperationException("used not correctly");

            var filter = Builders<Product>.Filter.Eq(oldPro => oldPro.ProductId, newProduct.ProductId);
            var updateResult = sessionIsNull ? _context.ProductCollection.ReplaceOne(filter, newProduct) :
                                               _context.ProductCollection.ReplaceOne(session: sessionHandle, filter, newProduct);
            if (updateResult.ModifiedCount > 0) {
                return true;
            }
            return false;
        }

        public bool Delete(Product productDel, IClientSessionHandle sessionHandle = null)
        {
            bool sessionIsNull = sessionHandle == null;
            if (!sessionIsNull && !sessionHandle.IsInTransaction)
                throw new InvalidOperationException("used not correctly");

            var filter = Builders<Product>.Filter.Eq(oldPro => oldPro.ProductId, productDel.ProductId);
            var deleteResult = sessionIsNull ? _context.ProductCollection.DeleteOne(filter) :
                                               _context.ProductCollection.DeleteOne(session: sessionHandle, filter);
            if (deleteResult.DeletedCount > 0) {
                return true;
            }
            return false;
        }

        public bool SaveChanges(Product newProduct, IClientSessionHandle sessionHandle = null)
        {
            bool sessionIsNull = sessionHandle == null;
            if (!sessionIsNull && !sessionHandle.IsInTransaction)
                throw new InvalidOperationException("used not correctly");

            var filter = Builders<Product>.Filter.Eq(oldPro => oldPro.ProductId, newProduct.ProductId);
            var updateResult = sessionIsNull ? _context.ProductCollection.ReplaceOne(filter, newProduct) :
                                               _context.ProductCollection.ReplaceOne(session: sessionHandle, filter, newProduct);
            if (updateResult.ModifiedCount > 0) {
                return true;
            }
            return false;
        }
        #endregion

        #region Async Methods
        public async Task<Product> GetAsync(Guid id)
        {
            Product result = await _context.ProductCollection.Find(p => p.ProductId == id).FirstOrDefaultAsync();
            return result;
        }

        public async Task AddAsync(Product product, IClientSessionHandle sessionHandle = null)
        {
            if (sessionHandle == null) {
                await _context.ProductCollection.InsertOneAsync(product);
                return;
            }
            if (!sessionHandle.IsInTransaction) throw new InvalidOperationException("used not correctly");
            await _context.ProductCollection.InsertOneAsync(session: sessionHandle, product);
        }

        public async Task<bool> ReplaceAsync(Product newProduct, IClientSessionHandle sessionHandle = null)
        {
            bool sessionIsNull = sessionHandle == null;
            if (!sessionIsNull && !sessionHandle.IsInTransaction)
                throw new InvalidOperationException("used not correctly");

            var filter = Builders<Product>.Filter.Eq(oldPro => oldPro.ProductId, newProduct.ProductId);
            var updateResult = sessionIsNull ? await _context.ProductCollection.ReplaceOneAsync(filter, newProduct) :
                                               await _context.ProductCollection.ReplaceOneAsync(session: sessionHandle, filter, newProduct);
            if (updateResult.ModifiedCount > 0) {
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteAsync(Product productDel, IClientSessionHandle sessionHandle = null)
        {
            bool sessionIsNull = sessionHandle == null;
            if (!sessionIsNull && !sessionHandle.IsInTransaction)
                throw new InvalidOperationException("used not correctly");

            var filter = Builders<Product>.Filter.Eq(oldPro => oldPro.ProductId, productDel.ProductId);
            var deleteResult = sessionIsNull ? await _context.ProductCollection.DeleteOneAsync(filter) :
                                               await _context.ProductCollection.DeleteOneAsync(session: sessionHandle, filter);
            if (deleteResult.DeletedCount > 0) {
                return true;
            }
            return false;
        }

        public async Task<bool> SaveChangesAsync(Product newProduct, IClientSessionHandle sessionHandle = null)
        {
            bool sessionIsNull = sessionHandle == null;
            if (!sessionIsNull && !sessionHandle.IsInTransaction)
                throw new InvalidOperationException("used not correctly");

            var filter = Builders<Product>.Filter.Eq(oldPro => oldPro.ProductId, newProduct.ProductId);
            var updateResult = sessionIsNull ? await _context.ProductCollection.ReplaceOneAsync(filter, newProduct) :
                                               await _context.ProductCollection.ReplaceOneAsync(session: sessionHandle, filter, newProduct);
            if (updateResult.ModifiedCount > 0) {
                return true;
            }
            return false;
        }
        #endregion

        public void SaveChanges(IClientSessionHandle sessionHandle = null)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Product> GetAllAsQueryable()
        {
            var result = _context.ProductCollection.AsQueryable();
            return result;
        }
    }
}
