namespace eShopAnalysis.Aggregator.Services.BackchannelDto
{
    /// <summary>
    /// request from aggregate read to ProductCatalog 
    /// to obtain more info about product model like image, product name, price, isOnSale, ...
    /// in Aggregate read GetProductModelInfosWithStockOfProvider
    /// </summary>
    public class ProductModelInfoRequestMetaDto
    {
        public Guid ProductId { get; set; }

        public Guid ProductModelId { get; set; }

        public Guid BusinessKey { get; set; }

        public double UnitRequestPrice { get; set; }
    }
}
