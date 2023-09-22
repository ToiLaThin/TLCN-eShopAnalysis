using System.Diagnostics.Metrics;
using System.IO;
using System.Reflection.Emit;

namespace eShopAnalysis.ProductCatalogAPI.Domain.Models
{
    public class ProductInfo //value object
    {
        public string ProductDescription { get; set; }
        public string ProductBrand{ get; set; }

        public ProductInfo(string description, string brand)
        {
            ProductDescription = description;
            ProductBrand = brand;
        }

        public ProductInfo() { } 

        protected IEnumerable<object> GetEqualityComponents()
        {
            // Using a yield return statement to return each element one at a time
            yield return ProductDescription;
            yield return ProductBrand;
        }

    }
}
