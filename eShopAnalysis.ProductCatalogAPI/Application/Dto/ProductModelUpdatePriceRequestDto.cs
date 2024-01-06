namespace eShopAnalysis.ProductCatalogAPI.Application.Dto
{
    public class ProductModelUpdatePriceRequestDto
    {
        public Guid ProductId { get; set; }

        public Guid ProductModelId { get; set; }

        public double NewPrice { get; set; }

    }
}
