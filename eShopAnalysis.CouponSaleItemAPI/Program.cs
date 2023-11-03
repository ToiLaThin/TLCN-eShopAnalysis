using eShopAnalysis.EventBus.Abstraction;
using eShopAnalysis.EventBus.Extension;
using eShopAnalysis.CouponSaleItemAPI.Data;
using Microsoft.EntityFrameworkCore;
using eShopAnalysis.CouponSaleItemAPI.UnitOfWork;
using eShopAnalysis.CouponSaleItemAPI.Service;
using AutoMapper;
using System.Reflection;
using eShopAnalysis.CouponSaleItemAPI.Utilities;
using eShopAnalysis.CouponSaleItemAPI.Service.BackchannelService;
using eShopAnalysis.CouponSaleItemAPI.Service.BackChannelService;
using eShopAnalysis.CouponSaleItemAPI.Application.IntegrationEvents;
using Serilog;
using eShopAnalysis.CouponSaleItemAPI.Utilities.Behaviors;

var builder = WebApplication.CreateBuilder(args);
//this will config all required by event bus, review appsettings.json EventBus section and EventBus Connection string
//new just subscribe integration event and integration event handler
builder.Services.AddEventBus(builder.Configuration);
builder.Services.AddTransient<IIntegrationEventHandler<UserAppliedCouponToCartIntegrationEvent>, UserAppliedCouponToCartIntegrationEventHandling>();
builder.Host.UseSerilog((context, config) => {
    config.ReadFrom.Configuration(context.Configuration);
});
builder.Services.AddScoped<LoggingBehaviorActionFilter>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<PostgresDbContext>(ctxOption =>
{
    ctxOption.UseNpgsql(builder.Configuration.GetConnectionString("CouponSaleItemConnString"));
});
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<ISaleItemService, SaleItemService>();
builder.Services.AddHttpClient(); //resolve http client factory
builder.Services.AddScoped<IBackChannelProductCatalogService, BackChannelProductCatalogService>();
builder.Services.AddScoped(typeof(IBackChannelBaseService<,>), typeof(BackChannelBaseService<,>));
//config automapper
var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddMaps(assembliesToScan: Assembly.GetExecutingAssembly());
});
IMapper mapper = new Mapper(mapperConfig);
builder.Services.AddSingleton(mapper);

builder.Services.Configure<BackChannelCommunication>(builder.Configuration.GetSection(nameof(BackChannelCommunication)));

var app = builder.Build();
app.UseSerilogRequestLogging();
var eventBus = app.Services.GetRequiredService<IEventBus>();
eventBus.Subscribe<UserAppliedCouponToCartIntegrationEvent, IIntegrationEventHandler<UserAppliedCouponToCartIntegrationEvent>>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthorization();
app.MapControllers();
app.Run();
