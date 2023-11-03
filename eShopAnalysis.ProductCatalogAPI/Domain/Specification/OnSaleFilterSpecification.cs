using eShopAnalysis.ProductCatalogAPI.Domain.Models.Aggregator;
using eShopAnalysis.ProductCatalogAPI.Domain.Specification;

namespace eShopAnalysis.ProductCatalogAPI.Domain.Specification
{
    public class OnSaleFilterSpecification: BaseFilterSpecification<Product>
    {
        public OnSaleFilterSpecification() 
            : base(p => p.IsOnSale)
        {
            
        }
    }
}
