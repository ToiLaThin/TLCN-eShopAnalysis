using eShopAnalysis.CouponSaleItemAPI.Service;
using eShopAnalysis.EventBus.Abstraction;

namespace eShopAnalysis.CouponSaleItemAPI.Application.IntegrationEvents
{
    public class UserAppliedCouponToCartIntegrationEventHandling : IIntegrationEventHandler<UserAppliedCouponToCartIntegrationEvent>
    {
        private readonly ICouponService _couponService;
        public UserAppliedCouponToCartIntegrationEventHandling(ICouponService couponService)
        {
            _couponService = couponService ?? throw new ArgumentNullException(nameof(couponService));
        }
        //la async
        public async Task Handle(UserAppliedCouponToCartIntegrationEvent @event)
        {
            var result = await _couponService.MarkUserUsedCoupon(userId: @event.UserId, couponId: @event.CouponId);
            if (result.IsFailed)
            {
                throw new Exception("Please implement a outbox pattern to make sure automicity in distributed transaction");
                //TODO add event store and background service(following outbox pattern) to make sure if this failed, the integration event can be republish and have idempotence
            }
            var data = result.Data;
            return;
        }
    }
}
