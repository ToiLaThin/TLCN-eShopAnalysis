using eShopAnalysis.Aggregator.Services.BackchannelDto;

namespace eShopAnalysis.Aggregator.ClientDto
{
    /// <summary>
    /// request from Client 
    /// to confirm cart (check coupon) & (add cart)
    /// </summary>
    public class CartConfirmRequestDto
    {
        public IEnumerable<CartItem> CartItems { get; set; }

        public Guid UserId { get; set; }

        //not require
        public string? CouponCode { get; set; }
    }
}
