using eShopAnalysis.EventBus.Abstraction;
using Newtonsoft.Json;

namespace eShopAnalysis.IdentityServer.IntegrationEvents.Event
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
