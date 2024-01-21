namespace eShopAnalysis.Aggregator.Services.BackchannelDto
{
    //request To CouponSaleItemAPI
    //to get coupon with a code
    //in aggregate CheckCouponAndAddCart
    public class RetrieveCouponWithCodeRequestDto
    {
        public string CouponCode { get; set; }
    }
}
