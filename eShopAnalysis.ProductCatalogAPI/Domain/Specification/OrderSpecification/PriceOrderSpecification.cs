using eShopAnalysis.ProductCatalogAPI.Domain.Models.Aggregator;
using System.Linq.Expressions;

namespace eShopAnalysis.ProductCatalogAPI.Domain.Specification
{
    public class PriceOrderSpecification : BaseOrderSpecification<Product>
    {
        public PriceOrderSpecification(OrderType orderType): base(p => p.ProductModels[0].Price, orderType) { }
    }
}
