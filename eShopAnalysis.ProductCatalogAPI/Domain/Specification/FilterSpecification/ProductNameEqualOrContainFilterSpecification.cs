using eShopAnalysis.ProductCatalogAPI.Domain.Models.Aggregator;

namespace eShopAnalysis.ProductCatalogAPI.Domain.Specification.FilterSpecification
{
    public class ProductNameEqualOrContainFilterSpecification: BaseFilterSpecification<Product>
    {
        public ProductNameEqualOrContainFilterSpecification(string productNameSearchNormalized) : base(p =>
            p.ProductName.Trim().ToLower().Equals(productNameSearchNormalized) || 
            p.ProductName.Trim().ToLower().Contains(productNameSearchNormalized)
        ) {}
    }
}
