using eShopAnalysis.CartOrderAPI.Application.BackchannelDto;
using eShopAnalysis.CartOrderAPI.Domain.DomainModels.CartAggregate;

namespace eShopAnalysis.CartOrderAPI.Application.Dto
{
    public class CartConfirmRequestToCartApiDto
    {
        public IEnumerable<CartItem> CartItems { get; set; }

        public Guid UserId { get; set; }

        //not require
        public CouponDto? CouponCode { get; set; }
    }
}
