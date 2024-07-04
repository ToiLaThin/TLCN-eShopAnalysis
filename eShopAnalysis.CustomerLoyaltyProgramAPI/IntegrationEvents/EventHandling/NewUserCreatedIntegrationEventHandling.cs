using eShopAnalysis.CustomerLoyaltyProgramAPI.IntegrationEvents.Event;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Service;
using eShopAnalysis.EventBus.Abstraction;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.IntegrationEvents.EventHandling
{
    public class NewUserCreatedIntegrationEventHandling : IIntegrationEventHandler<NewUserCreatedIntegrationEvent>
    {
        private readonly IUserRewardPointService _userRewardPointService;

        public NewUserCreatedIntegrationEventHandling(IUserRewardPointService userRewardPointService)
        {
            _userRewardPointService = userRewardPointService;
        }

        public async Task Handle(NewUserCreatedIntegrationEvent @event)
        {
            var serviceResult = await _userRewardPointService.AddInstance(@event.UserId, 0);
            if (serviceResult.IsFailed) {
                await Task.FromException(new Exception(serviceResult.Error));
            }
            await Task.FromResult("completed");
        }
    }
}
