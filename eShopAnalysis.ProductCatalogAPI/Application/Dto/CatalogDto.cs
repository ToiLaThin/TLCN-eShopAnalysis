namespace eShopAnalysis.ProductCatalogAPI.Application.Dto
{
    public class CatalogDto
    {
        public Guid CatalogId { get; set; }

        public string CatalogName { get; set; }

        public string CatalogDescription { get; set; }

        public string CatalogImage { get; set; }

        public List<SubCatalogDto> SubCatalogs { get; set; }
    }

    public class SubCatalogDto
    {
        public Guid SubCatalogId { get; set; }

        public string SubCatalogName { get; set; }

        public string SubCatalogDescription { get; set; }

        public string SubCatalogImage { get; set; }

    }
}
