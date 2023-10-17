using eShopAnalysis.EventBus.Abstraction;
using Newtonsoft.Json;

namespace eShopAnalysis.CartOrderAPI.Application.IntegrationEvents
{
    //sent to CouponSaleItem API to mark user applied this coupon
    public record UserAppliedCouponToCartIntegrationEvent: IntegrationEvent
    {
        [JsonProperty]
        public Guid UserId { get; }

        [JsonProperty]
        public Guid CouponId { get; }

        [JsonConstructor]
        public UserAppliedCouponToCartIntegrationEvent(Guid userId, Guid couponId)
        {
            UserId = userId;
            CouponId = couponId;
        }
    }
}
