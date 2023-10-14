using eShopAnalysis.CouponSaleItemAPI.Models;

namespace eShopAnalysis.CouponSaleItemAPI.Dto.BackchannelDto
{
    public class ProductUpdateToSaleRequestDto
    {
        //TODO fix the model of product and saleiitem, about the discount type
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
