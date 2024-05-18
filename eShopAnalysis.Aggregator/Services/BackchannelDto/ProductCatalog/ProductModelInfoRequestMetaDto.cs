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

        //these 2 will be send to ProductCatalogApi to do NOTHING, they 're reduntdant
        public int QuantityToRequestMoreFromProvider { get; set; }

        public int QuantityToNotify { get; set; }
    }
}
