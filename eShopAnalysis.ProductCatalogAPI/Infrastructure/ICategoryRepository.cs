using eShopAnalysis.ProductCatalogAPI.Domain.Models;
using eShopAnalysis.ProductCatalogAPI.Infrastructure.Data;
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

        Catalog Add(Catalog catalog);

        bool Remove(Guid catalog);
        bool Update(Catalog catalog);
    }
}
