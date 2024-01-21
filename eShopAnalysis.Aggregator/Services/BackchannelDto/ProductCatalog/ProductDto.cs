
namespace eShopAnalysis.Aggregator.Services.BackchannelDto
{
    //
    /// <summary>
    /// is used from Client to Aggregate & Aggregate to BackChannel ProductCatalog request to add product && response from update product to on sale from ProductCatalog
    /// is response from Aggregate to Client also
    /// </summary>
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

        public ProductDto()
        {
            //productId will be gen in mongo since it is id
            //default values
            HaveVariants = false;
            HavePricePerCublic = false;

            IsOnSale = false;
            ProductDisplaySaleValue = -1;
            ProductDisplaySaleType = DiscountType.NoDiscount;
            ProductDisplayPriceOnSale = -1;

            BusinessKey = Guid.NewGuid();
            Revision = 0;
        }
    }

    public class ProductInfoDto
    {
        public string ProductDescription { get; set; }
        public string ProductBrand { get; set; }

        public string ProductUsageInstruction { get; set; }

        public string ProductPreserveInstruction { get; set; }
        public string ProductIngredients { get; set; }
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

        public ProductModelDto()
        {
            ProductModelId = Guid.NewGuid();
            //default value
            IsOnSaleModel = false;
            SaleItemId = null; //at first it is null
            SaleType = DiscountType.NoDiscount;// them 1 discount Type là none cả trên front end và backend
            PriceOnSaleModel = -1;
            SaleValueModel = -1;

            PricePerCublicValue = -1;
            CublicValue = -1;
            CublicType = CublicType.N;
        }
    }
}
