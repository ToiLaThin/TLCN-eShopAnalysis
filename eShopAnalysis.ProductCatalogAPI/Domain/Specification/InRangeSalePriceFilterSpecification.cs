using eShopAnalysis.ProductCatalogAPI.Domain.Models.Aggregator;
using eShopAnalysis.ProductCatalogAPI.Domain.Specification;

namespace eShopAnalysis.ProductCatalogAPI.Domain.Specification
{
    public class InRangeSalePriceFilterSpecification: BaseFilterSpecification<Product>
    {
        public InRangeSalePriceFilterSpecification(double low, double high)
            : base(p => p.ProductDisplayPriceOnSale >= low && p.ProductDisplayPriceOnSale <= high)
        {
            
        }
    }
}
