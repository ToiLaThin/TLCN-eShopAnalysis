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

        public Product Get(Guid id)
        {
            Product result = _context.ProductCollection.AsQueryable().Where(p => p.ProductId== id).FirstOrDefault();
            return result;
        }

        public IEnumerable<Product> GetAll()
        {
            var result = _context.ProductCollection.AsQueryable().ToList();
            return result;
        }

        public IQueryable<Product> GetAllAsQueryable()
        {
            var result = _context.ProductCollection.AsQueryable();
            return result;
        }

        public void Add(Product product, IClientSessionHandle sessionHandle = null)
        {
            if (sessionHandle != null) {
                if (!sessionHandle.IsInTransaction) throw new InvalidOperationException("used not correctly");
                _context.ProductCollection.InsertOne(session: sessionHandle, product);
            }
            else {
                _context.ProductCollection.InsertOne(product);
            }
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
            else { return false; }
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
            else { return false; }
        }

        public bool SaveChanges(Product newProduct, IClientSessionHandle sessionHandle = null)
        {
            bool sessionIsNull = sessionHandle == null;
            if (!sessionIsNull && !sessionHandle.IsInTransaction)
                throw new InvalidOperationException("used not correctly");

            var filter = Builders<Product>.Filter.Eq(oldPro => oldPro.ProductId, newProduct.ProductId);
            var updateResult = sessionIsNull ? _context.ProductCollection.ReplaceOne(filter, newProduct) :
                                               _context.ProductCollection.ReplaceOne(session: sessionHandle, filter, newProduct);
            if (updateResult.ModifiedCount > 0)
            {
                return true;
            }
            else { return false; }
        }

        public void SaveChanges(IClientSessionHandle sessionHandle = null)
        {
            throw new NotImplementedException();
        }
    }
}
