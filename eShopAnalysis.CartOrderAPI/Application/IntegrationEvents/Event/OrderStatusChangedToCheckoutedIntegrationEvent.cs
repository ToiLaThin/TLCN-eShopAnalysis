using eShopAnalysis.CartOrderAPI.Domain.DomainModels.OrderAggregate;
using eShopAnalysis.EventBus.Abstraction;
using Newtonsoft.Json;

namespace eShopAnalysis.CartOrderAPI.Application.IntegrationEvents.Event
{
    //this integration event 's sent to notification hub to notify user they paid successfully a amount
    public record OrderStatusChangedToCheckoutedIntegrationEvent: IntegrationEvent
    {
        [JsonProperty]
        public Guid OrderId { get; }

        [JsonProperty]
        public Guid UserId { get; }

        [JsonProperty]
        public double PaidAmount { get; }

        [JsonProperty]
        public PaymentMethod PaymentMethod { get; }

        [JsonProperty]
        public DateTime DateCheckouted { get; }

        [JsonConstructor]
        public OrderStatusChangedToCheckoutedIntegrationEvent(Guid orderId, Guid userId, PaymentMethod paymentMethod, DateTime dateCheckouted, double paidAmount)
        {
            OrderId = orderId;
            UserId = userId;
            PaymentMethod = paymentMethod;
            PaidAmount = paidAmount;
            DateCheckouted = dateCheckouted;
        }
    }
}
