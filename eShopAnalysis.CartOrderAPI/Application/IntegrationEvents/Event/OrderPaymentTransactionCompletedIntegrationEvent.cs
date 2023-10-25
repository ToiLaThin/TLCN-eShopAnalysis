using eShopAnalysis.CartOrderAPI.Domain.DomainModels.OrderAggregate;
using eShopAnalysis.EventBus.Abstraction;
using Newtonsoft.Json;

namespace eShopAnalysis.CartOrderAPI.IntegrationEvents
{
    //received from payment API to handle changing order status and payment method
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
