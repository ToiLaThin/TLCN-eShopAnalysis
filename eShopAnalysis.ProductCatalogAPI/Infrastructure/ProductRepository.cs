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

        public Product Add(Product product)
        {
            _context.ProductCollection.InsertOne(product);
            Product resultIns = _context.ProductCollection.Find(c => c.ProductId == product.ProductId).FirstOrDefault();
            return resultIns;
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

        public bool Replace(Product newProduct)
        {
            var filter = Builders<Product>.Filter.Eq(oldPro => oldPro.ProductId, newProduct.ProductId);

            var updateResult = _context.ProductCollection.ReplaceOne(filter, newProduct);
            if (updateResult.ModifiedCount > 0)
            {
                return true;
            }
            else { return false; }
        }

        public bool Delete(Product productDel)
        {
            var filter = Builders<Product>.Filter.Eq(oldPro => oldPro.ProductId, productDel.ProductId);
            var deleteResult = _context.ProductCollection.DeleteOne(filter);
            if (deleteResult.DeletedCount > 0)
            {
                return true;
            }
            else { return false; }
        }

        public bool SaveChanges(Product newProduct)
        {
            var filter = Builders<Product>.Filter.Eq(oldPro => oldPro.ProductId, newProduct.ProductId);

            var updateResult = _context.ProductCollection.ReplaceOne(filter, newProduct);
            if (updateResult.ModifiedCount > 0)
            {
                return true;
            }
            else { return false; }
        }

        public void SaveChanges()
        {
            throw new NotImplementedException();
        }
    }
}
