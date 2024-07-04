using eShopAnalysis.EventBus.Abstraction;
using Newtonsoft.Json;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.IntegrationEvents.Event
{
    public record NewUserCreatedIntegrationEvent: IntegrationEvent
    {
        [JsonProperty]
        public Guid UserId { get; }

        [JsonConstructor]
        public NewUserCreatedIntegrationEvent(Guid userId)
        {
            UserId = userId;
        }
    }
}
