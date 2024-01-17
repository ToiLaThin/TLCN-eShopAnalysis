namespace eShopAnalysis.ProductCatalogAPI.Application.BackChannelDto
{
    /// <summary>
    /// sent from aggregate read to ProductCatalog to obtain more info about product model like image, product name, price, isOnSale, ...
    /// </summary>
    public class ProductModelInfoRequestMetaDto
    {
        public Guid ProductId { get; set; }

        public Guid ProductModelId { get; set; }

        public Guid BusinessKey { get; set; }

        public double UnitRequestPrice { get; set; }
    }
}
