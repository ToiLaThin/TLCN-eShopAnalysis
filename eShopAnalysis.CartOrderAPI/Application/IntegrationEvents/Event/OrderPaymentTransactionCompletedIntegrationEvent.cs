using eShopAnalysis.CartOrderAPI.Domain.DomainModels.OrderAggregate;
using eShopAnalysis.EventBus.Abstraction;
using Newtonsoft.Json;

namespace eShopAnalysis.CartOrderAPI.IntegrationEvents
{
    //sent to CouponSaleItem API to mark user applied this coupon
    public record OrderPaymentTransactionCompletedIntegrationEvent: IntegrationEvent
    {
        [JsonProperty]
        public Guid OrderId { get; }

        [JsonProperty]
        public PaymentMethod PaymentMethod { get; }

        [JsonProperty]
        public DateTime DateCheckouted { get; }

        [JsonConstructor]
        public OrderPaymentTransactionCompletedIntegrationEvent(Guid orderId, PaymentMethod paymentMethod)
        {
            OrderId = orderId;
            PaymentMethod = paymentMethod;
            DateCheckouted = DateTime.UtcNow;
        }
    }
}
