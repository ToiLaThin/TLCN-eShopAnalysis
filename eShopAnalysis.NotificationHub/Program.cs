
using eShopAnalysis.CartOrderAPI.Application.IntegrationEvents.Event;
using eShopAnalysis.EventBus.Abstraction;
using eShopAnalysis.EventBus.Extension;
using eShopAnalysis.NotificationHub;
using eShopAnalysis.NotificationHub.IntegrationEvents.EventHandling;
using eShopAnalysis.NotificationHub.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNet.SignalR;
using eShopAnalysis.NotificationHub.Application.IntegrationEvents.Event;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEventBus(builder.Configuration);
builder.Services.AddTransient<IIntegrationEventHandler<OrderStatusChangedToCheckoutedIntegrationEvent>, OrderStatusChangedToCheckoutedIntegrationEventHandling>();
builder.Services.AddTransient<IIntegrationEventHandler<ProductModelPriceUpdatedIntegrationEvent>, ProductModelPriceUpdatedIntegrationEventHandling>();
string hubCorPolicyKey = "NotificationHubCorPolicy";
builder.Services.AddCors(corsOption =>
{
    corsOption.AddPolicy(hubCorPolicyKey, p => p.WithOrigins("http://localhost:4200")
                                                .AllowAnyMethod()
                                                .AllowAnyHeader()
                                                .AllowCredentials()
                                              );
});
builder.Services.AddAuthentication()
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwtOption =>
                {
                    jwtOption.Authority = builder.Configuration.GetSection("OpenIdConnectAuthority:IdentityServerBaseUri").Value.ToString();
                    jwtOption.SaveToken = true;
                    jwtOption.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = false,
                        ValidateActor = false,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    };
                    jwtOption.RequireHttpsMetadata = false;
                    jwtOption.Events = new JwtBearerEvents {
                        OnMessageReceived = (msgContext) =>
                        {
                            var accessToken = msgContext.Request.Query["access_token"];
                            var requestPath = msgContext.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && (requestPath.StartsWithSegments("/NotificationHub"))) {
                                msgContext.Token = accessToken;
                            }
                            msgContext.HttpContext.Connection.Id = Guid.NewGuid().ToString();
                            return Task.CompletedTask; 
                        }
                    };
                });

builder.Services.AddAuthorization(authOption =>
{
    authOption.AddPolicy(PolicyNames.AdminPolicy, policy =>
    {
        policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
        policy.RequireRole(RoleType.Admin);
    });
    authOption.AddPolicy(PolicyNames.AuthenticatedUserPolicy, policy =>
    {
        policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
        policy.RequireRole(RoleType.AuthenticatedUser);
    });
});
//from signalR to resolve IUserIdProvider implementation is FromSubSignalRUserIdProvider
GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => new FromSubSignalRUserIdProvider());
builder.Services.AddSignalR();
builder.Services.AddControllers();
var app = builder.Build();
var eventBus = app.Services.GetRequiredService<IEventBus>();
eventBus.Subscribe<OrderStatusChangedToCheckoutedIntegrationEvent, IIntegrationEventHandler<OrderStatusChangedToCheckoutedIntegrationEvent>>();
eventBus.Subscribe<ProductModelPriceUpdatedIntegrationEvent, IIntegrationEventHandler<ProductModelPriceUpdatedIntegrationEvent>>();
app.MapHub<NotificationHub>("/NotificationHub", signalROptions =>
{
    signalROptions.Transports = HttpTransportType.WebSockets | HttpTransportType.ServerSentEvents;
    signalROptions.CloseOnAuthenticationExpiration = true;
});
app.UseCors(hubCorPolicyKey);//must be between useAuthen to avoid cors error
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
