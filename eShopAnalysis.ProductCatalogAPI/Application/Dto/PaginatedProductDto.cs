namespace eShopAnalysis.ProductCatalogAPI.Application.Dto
{
    public class PaginatedProductDto
    {
        public IEnumerable<ProductDto> Products { get; set; }

        public int PageNumber { get; set; }

        public int PageCount { get; set; }

    }
}
