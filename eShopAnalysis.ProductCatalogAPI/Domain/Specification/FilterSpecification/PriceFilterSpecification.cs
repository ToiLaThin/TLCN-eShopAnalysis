using eShopAnalysis.ProductCatalogAPI.Domain.Models.Aggregator;

namespace eShopAnalysis.ProductCatalogAPI.Domain.Specification
{
    public class PriceFilterSpecification: BaseFilterSpecification<Product>
    {
        public PriceFilterSpecification(double fromPrice, double toPrice): base(p => p.ProductModels[0].Price >= fromPrice && p.ProductModels[0].Price <= toPrice) { }
    }
}
