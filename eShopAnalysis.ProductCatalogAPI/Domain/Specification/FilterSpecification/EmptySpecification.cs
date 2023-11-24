using eShopAnalysis.ProductCatalogAPI.Domain.Models.Aggregator;

namespace eShopAnalysis.ProductCatalogAPI.Domain.Specification
{
    public class EmptySpecification: BaseFilterSpecification<Product>
    {
        public EmptySpecification(): base(p => true) { }
    }
}
