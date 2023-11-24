using eShopAnalysis.ProductCatalogAPI.Domain.Models.Aggregator;

namespace eShopAnalysis.ProductCatalogAPI.Domain.Specification
{
    public class ProductNameOrderSpecification: BaseOrderSpecification<Product>
    {
        public ProductNameOrderSpecification(OrderType orderType): base(p => p.ProductName, orderType) {}
    }
}
