using eShopAnalysis.ProductCatalogAPI.Domain.Models.Aggregator;

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
