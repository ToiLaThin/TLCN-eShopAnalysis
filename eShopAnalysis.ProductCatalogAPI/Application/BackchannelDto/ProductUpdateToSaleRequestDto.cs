using static eShopAnalysis.ProductCatalogAPI.Domain.Models.ProductModel;

namespace eShopAnalysis.ProductCatalogAPI.Application.BackChannelDto
{
    //from aggregator to update product to on sale
    public class ProductUpdateToSaleRequestDto
    {
        public Guid ProductId { get; set; }

        public Guid ProductModelId { get; set; }

        public Guid SaleItemId { get; set; }

        public DiscountType DiscountType { get; set; }

        public double DiscountValue { get; set; } 
        //the value can be percent or value
        //if percent, it is from 1 -> 100%
        //if value , must be less than the product model price, but we can enforce these constraint later

    }
}
