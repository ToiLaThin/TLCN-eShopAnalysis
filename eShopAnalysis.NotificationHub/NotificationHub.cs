using eShopAnalysis.NotificationHub.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace eShopAnalysis.NotificationHub
{
    public static class EventsToSentClient
    {
        public const string OrderStatusChanged = "OrderStatusChanged";
        public const string ProductModelPriceChanged = "ProductModelPriceChanged";
    }

    [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = PolicyNames.AuthenticatedUserPolicy,
            Roles = RoleType.AuthenticatedUser
           )
    ]
    //https://learn.microsoft.com/en-us/aspnet/core/signalr/configuration?view=aspnetcore-7.0&tabs=dotnet
    //https://stackoverflow.com/questions/48119106/signalr-angular-how-to-add-bearer-token-to-http-headers need further config to work otherwise will not authorize
    //for Context.User.Identity.Name to have value
    public class NotificationHub: Hub
    {
        public override async Task OnConnectedAsync()
        {
            //add to group so we can send event to a specific user, also Context is HubContext not HttpContext
            //create a mapping between the connection and the user (you must specify how to identify a user )
            //by default signalR give this is Context.User.Identity.Name
            //https://stackoverflow.com/questions/19522103/signalr-sending-a-message-to-a-specific-user-using-iuseridprovider-new-2-0
            //https://learn.microsoft.com/en-us/aspnet/signalr/overview/guide-to-the-api/mapping-users-to-connections#IUserIdProvider
            await Groups.AddToGroupAsync(Context.ConnectionId, Context.UserIdentifier);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.UserIdentifier);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
