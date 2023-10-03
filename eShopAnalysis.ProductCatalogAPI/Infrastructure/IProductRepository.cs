using eShopAnalysis.ProductCatalogAPI.Domain.Models;
using eShopAnalysis.ProductCatalogAPI.Domain.Models.Aggregator;

namespace eShopAnalysis.ProductCatalogAPI.Infrastructure
{
    public interface IProductRepository : IRepository<Product>
    {
        Product Add(Product product);

        bool Replace(Product product);

        bool Delete(Product productDel);

        bool SaveChanges(Product product); //wrapper for mongo db replace

        Product Get(Guid id);
        IEnumerable<Product> GetAll();
        IQueryable<Product> GetAllAsQueryable();
    }
}
