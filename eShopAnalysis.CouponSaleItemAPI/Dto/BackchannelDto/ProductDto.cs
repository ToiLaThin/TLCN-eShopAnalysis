
namespace eShopAnalysis.Dto.BackchannelDto
{
    public enum CublicType
    {
        M, //khoi luowng
        V, //the tich
        S, //dien tich
        N //do not have cubic

    };

    //the return type after send the request to the productCatalog microservice to 
    //update the product to isSaleItem = true; and other props
    //just a return type to have loopback request
    public class ProductDto
    {

        public Guid ProductId { get; set; }

        public string ProductName { get; set; }

        public Guid SubCatalogId { get; set; }

        public string SubCatalogName { get; set; }

        public string ProductCoverImage { get; set; }

        public bool IsOnSale { get; set; } 

        public double SalePercent { get; set; } 

        public double PriceOnSale { get; set; }


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

        public double SalePercentModel { get; set; }

        public double PriceOnSaleModel { get; set; }
    }
}
