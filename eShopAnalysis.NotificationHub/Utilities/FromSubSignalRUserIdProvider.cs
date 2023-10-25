using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace eShopAnalysis.NotificationHub.Utilities
{
    public class FromSubSignalRUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            //https://stackoverflow.com/questions/57998262/why-is-claimtypes-nameidentifier-not-mapping-to-sub why nameidentifier not mapped to sub claim
            string userId = connection.User.FindFirstValue("sub") ?? throw new ArgumentNullException(nameof(connection));
            return userId;
        }
    }
}
