using eShopAnalysis.ProductCatalogAPI.Domain.Models;
using eShopAnalysis.ProductCatalogAPI.Domain.Models.Aggregator;
using MongoDB.Driver;

namespace eShopAnalysis.ProductCatalogAPI.Infrastructure
{
    public interface IProductRepository : IRepository<Product>
    {
        //if we pass in the sessionHandle, we make it a part of transaction
        void Add(Product product, IClientSessionHandle sessionHandle = null);

        bool Replace(Product product, IClientSessionHandle sessionHandle = null);

        bool Delete(Product productDel, IClientSessionHandle sessionHandle = null);

        bool SaveChanges(Product product, IClientSessionHandle sessionHandle = null); //wrapper for mongo db replace

        Product Get(Guid id);

        Task AddAsync(Product product, IClientSessionHandle sessionHandle = null);

        Task<bool> ReplaceAsync(Product product, IClientSessionHandle sessionHandle = null);

        Task<bool> DeleteAsync(Product productDel, IClientSessionHandle sessionHandle = null);

        Task<bool> SaveChangesAsync(Product product, IClientSessionHandle sessionHandle = null); //wrapper for mongo db replace

        Task<Product> GetAsync(Guid id);

        IQueryable<Product> GetAllAsQueryable();
    }
}
