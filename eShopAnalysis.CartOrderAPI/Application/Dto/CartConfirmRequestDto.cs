using eShopAnalysis.CartOrderAPI.Domain.DomainModels.CartAggregate;

namespace eShopAnalysis.CartOrderAPI.Application.Dto
{
    public class CartConfirmRequestDto
    {
        public IEnumerable<CartItem> CartItems { get; set; }

        public Guid UserId { get; set; }

        //not require
        public string? CouponCode { get; set; }
    }
}
