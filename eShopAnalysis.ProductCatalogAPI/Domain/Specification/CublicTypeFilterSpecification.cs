using eShopAnalysis.ProductCatalogAPI.Domain.Models;
using eShopAnalysis.ProductCatalogAPI.Domain.Models.Aggregator;
using eShopAnalysis.ProductCatalogAPI.Domain.Specification;
using System.Linq.Expressions;

namespace eShopAnalysis.ProductCatalogAPI.Domain.Specification
{
    public class CublicTypeFilterSpecification: BaseFilterSpecification<Product>
    {
        public CublicTypeFilterSpecification(
            CublicType cublicType) : base(p => p.ProductModels.Any(pm => pm.CublicType == cublicType))
        {
        }

    }
}
