using eShopAnalysis.ProductCatalogAPI.Domain.Models.Aggregator;

namespace eShopAnalysis.ProductCatalogAPI.Domain.Specification
{
    public class SaleDisplayPriceOrderSpecification : BaseOrderSpecification<Product>
    {
        public SaleDisplayPriceOrderSpecification(
            OrderType orderType) : base(p => p.ProductDisplayPriceOnSale, orderType)
        {
        }
    }
}
