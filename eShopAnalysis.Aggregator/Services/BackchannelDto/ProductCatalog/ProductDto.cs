
namespace eShopAnalysis.Aggregator.Services.BackchannelDto
{
    //response from update product to on sale from ProductCatalog
    public class ProductDto
    {
        public Guid ProductId { get; set; }

        public string ProductName { get; set; }

        public Guid SubCatalogId { get; set; }

        public string SubCatalogName { get; set; }

        public string ProductCoverImage { get; set; }

        public bool IsOnSale { get; set; } 

        public double ProductDisplaySaleValue { get; set; }

        public DiscountType ProductDisplaySaleType { get; set; }

        public double ProductDisplayPriceOnSale { get; set; }


        public bool HaveVariants { get; set; }

        public bool HavePricePerCublic { get; set; }

        public int Revision { get; set; }

        public Guid BusinessKey { get; set; }

        public ProductInfoDto ProductInfo { get; set; }

        public List<ProductModelDto> ProductModels { get; set; }
    }

    public class ProductInfoDto
    {
        public string ProductDescription { get; set; }
        public string ProductBrand { get; set; }
    }

    public enum CublicType
    {
        M, //khoi luowng
        V, //the tich
        S, //dien tich
        N //do not have cubic

    };

    public class ProductModelDto
    {
        public Guid ProductModelId { get; set; }

        public IEnumerable<string> ProductModelThumbnails { get; set; }

        public CublicType CublicType { get; set; }

        public double CublicValue { get; set; } 

        public double PricePerCublicValue { get; set; }
        public double CublicPrice { get; set; } 
        public double Price { get; set; }

        public bool IsOnSaleModel { get; set; }

        public Guid? SaleItemId { get; set; }

        public double SaleValueModel { get; set; }

        public DiscountType SaleType { get; set; }

        public double PriceOnSaleModel { get; set; }
    }
}
