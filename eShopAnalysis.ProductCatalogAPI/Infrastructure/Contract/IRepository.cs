using MongoDB.Driver;
using System.Linq.Expressions;

namespace eShopAnalysis.ProductCatalogAPI.Infrastructure
{
    public interface IRepository<TEntity> where TEntity : class
    {

        void SaveChanges(IClientSessionHandle sessionHandle = null);
    }
}
