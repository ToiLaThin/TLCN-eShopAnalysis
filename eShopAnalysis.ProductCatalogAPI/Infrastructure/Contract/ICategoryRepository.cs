using eShopAnalysis.ProductCatalogAPI.Domain.Models;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace eShopAnalysis.ProductCatalogAPI.Infrastructure
{
    public interface ICatalogRepository : IRepository<Catalog>
    {
        Catalog Get(Guid id);
        IEnumerable<Catalog> GetAll();
        IQueryable<Catalog> GetAllAsQueryable();
        IEnumerable<Catalog> Find(Expression<Func<Catalog, bool>> predicate);
        Catalog GetFirstEntity();

        //if adding with transaction, find will return null, because the record not really saved to db
        //so if used this with trans, change it to void and do not find the inserted one
        //this only work correctly in without transaction
        Catalog Add(Catalog catalog, IClientSessionHandle sessionHandle = null);

        //if adding with transaction, find will return null, because the record not really saved to db
        //so if used this with trans, change it to void and do not find the inserted one
        //this only work correctly in without transaction
        IEnumerable<Catalog> AddRange(IEnumerable<Catalog> catalogs, IClientSessionHandle sessionHandle = null);
        bool Remove(Guid catalog, IClientSessionHandle sessionHandle = null);
        bool Update(Catalog catalog, IClientSessionHandle sessionHandle = null);
    }
}
