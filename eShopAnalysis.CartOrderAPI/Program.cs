using eShopAnalysis.EventBus.Abstraction;
using eShopAnalysis.EventBus.Extension;
using eShopAnalysis.CartOrderAPI.Application.BackchannelDto;
using eShopAnalysis.CartOrderAPI.Application.BackchannelServices;
using eShopAnalysis.CartOrderAPI.Infrastructure;
using eShopAnalysis.CartOrderAPI.Infrastructure.Repositories;
using eShopAnalysis.CartOrderAPI.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
//this will config all required by event bus, review appsettings.json EventBus section and EventBus Connection string
//new just subscribe integration event and integration event handler
builder.Services.AddEventBus(builder.Configuration);
//builder.Services.AddTransient<IIntegrationEventHandler<GracePeriodConfirmedIntegrationEvent>, GracePeriodConfirmedIntegrationEventHandler>();

builder.Services.AddControllers();
builder.Services.AddDbContext<OrderCartContext>(ctxOption =>
{
    ctxOption.UseSqlServer(builder.Configuration.GetConnectionString("OrderCartDb"));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(mediatRConfig =>
{
    mediatRConfig.RegisterServicesFromAssembly(assembly: Assembly.GetExecutingAssembly());
});
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();

builder.Services.Configure<BackChannelCommunication>(builder.Configuration.GetSection(nameof(BackChannelCommunication)));
builder.Services.AddHttpClient(); //resolve IHttpClientFactory
builder.Services.AddScoped<IBackChannelBaseService<RetrieveCouponWithCodeRequestDto, CouponDto>, BackChannelBaseService<RetrieveCouponWithCodeRequestDto, CouponDto>>();
builder.Services.AddScoped<IBackChannelCouponSaleItemService, BackChannelCouponSaleItemService>();

var app = builder.Build();
var eventBus = app.Services.GetRequiredService<IEventBus>();
//eventBus.Subscribe<UserCheckoutAcceptedIntegrationEvent, IIntegrationEventHandler<UserCheckoutAcceptedIntegrationEvent>>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthorization();
app.MapControllers();
app.Run();
