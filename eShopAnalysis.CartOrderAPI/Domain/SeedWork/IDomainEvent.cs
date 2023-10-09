using MediatR;

namespace eShopAnalysis.CartOrderAPI.Domain.SeedWork
{
    //pub/sub mechanism
    //these domain event might be sent in the command handler
    public class IDomainEvent: INotification
    {
    }
}
