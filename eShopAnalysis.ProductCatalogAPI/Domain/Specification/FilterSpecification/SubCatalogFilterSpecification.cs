using eShopAnalysis.ProductCatalogAPI.Domain.Models.Aggregator;

namespace eShopAnalysis.ProductCatalogAPI.Domain.Specification
{
    public class SubCatalogFilterSpecification: BaseFilterSpecification<Product>
    {
        public SubCatalogFilterSpecification(IEnumerable<Guid> SubCatalogIds): base(p => SubCatalogIds.Contains(p.SubCatalogId)) { }
    }
}
