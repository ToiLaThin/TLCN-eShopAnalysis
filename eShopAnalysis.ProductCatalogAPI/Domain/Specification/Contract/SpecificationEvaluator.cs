using eShopAnalysis.ProductCatalogAPI.Application.Dto;

namespace eShopAnalysis.ProductCatalogAPI.Domain.Specification
{
    public static class SpecificationEvaluator<T>
    {
        public static IQueryable<T> GetQuery(
            IQueryable<T> query,
            IFilterSpecification<T>? filterSpec = null,
            IOrderSpecification<T>? orderSpec = null,
            IPaginateSpecification? paginateSpec = null
            )
        {
            // Modify the IQueryable
            // Apply filter conditions
            if (filterSpec?.Criterias != null)
            {
                filterSpec.Criterias.ForEach(c => {
                    query = query.Where(c);
                });
            }

            // Includes
            //query = specifications.Includes
            //            .Aggregate(query, (current, include) => current.Include(include));

            // Apply ordering
            if (orderSpec != null)
            {
                query = orderSpec.OrderType == OrderType.Ascending ? query.OrderBy(orderSpec.OrderBy)
                                                                    : query.OrderByDescending(orderSpec.OrderBy);
            }

            if (paginateSpec != null)
            {
                int skip = paginateSpec.PageSize * (paginateSpec.Page - 1);
                query = query.Skip(skip).Take(paginateSpec.PageSize);
            }

            return query;
        }
    }
}
