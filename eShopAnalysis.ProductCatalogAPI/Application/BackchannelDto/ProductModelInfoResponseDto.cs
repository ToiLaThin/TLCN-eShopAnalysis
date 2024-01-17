namespace eShopAnalysis.ProductCatalogAPI.Application.BackChannelDto
{
    /// <summary>
    /// Response from Product Catalog with additional info from ProductModelInfoRequestMetaDto
    /// </summary>
    public class ProductModelInfoResponseDto
    {
        public Guid ProductId { get; set; }

        public Guid BusinessKey { get; set; }

        public Guid ProductModelId { get; set; }


        //this is actually productName, but at product model level, we will still use this name
        public string ProductModelName { get; set; }

        public double Price { get; set; }

        public string ProductCoverImage { get; set; }
    }
}
