using eShopAnalysis.EventBus.Abstraction;
using eShopAnalysis.EventBus.Extension;
using eShopAnalysis.CouponSaleItemAPI.Data;
using Microsoft.EntityFrameworkCore;
using eShopAnalysis.CouponSaleItemAPI.UnitOfWork;
using eShopAnalysis.CouponSaleItemAPI.Service;
using AutoMapper;
using System.Reflection;
using eShopAnalysis.CouponSaleItemAPI.Application.IntegrationEvents;
using Serilog;
using eShopAnalysis.CouponSaleItemAPI.Utilities.Behaviors;
using eShopAnalysis.CouponSaleItemAPI.IntegrationEvents;

var builder = WebApplication.CreateBuilder(args);
//this will config all required by event bus, review appsettings.json EventBus section and EventBus Connection string
//new just subscribe integration event and integration event handler
builder.Services.AddEventBus(builder.Configuration);
builder.Services.AddTransient<IIntegrationEventHandler<UserAppliedCouponToCartIntegrationEvent>, UserAppliedCouponToCartIntegrationEventHandling>();
builder.Services.AddTransient<IIntegrationEventHandler<ProductModelPriceUpdatedIntegrationEvent>, ProductModelPriceUpdatedIntegrationEventHandling>();

builder.Host.UseSerilog((context, config) => {
    config.ReadFrom.Configuration(context.Configuration);
});
builder.Services.AddScoped<LoggingBehaviorActionFilter>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<PostgresDbContext>(ctxOption =>
{
    ctxOption.UseNpgsql(builder.Configuration.GetConnectionString("CouponSaleItemConnString"))
             .LogTo(Console.WriteLine,LogLevel.Information); //how to log db command in serilog
});
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<ISaleItemService, SaleItemService>();
//config automapper
var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddMaps(assembliesToScan: Assembly.GetExecutingAssembly());
});
IMapper mapper = new Mapper(mapperConfig);
builder.Services.AddSingleton(mapper);


var app = builder.Build();
app.UseSerilogRequestLogging();
var eventBus = app.Services.GetRequiredService<IEventBus>();
eventBus.Subscribe<UserAppliedCouponToCartIntegrationEvent, IIntegrationEventHandler<UserAppliedCouponToCartIntegrationEvent>>();
eventBus.Subscribe<ProductModelPriceUpdatedIntegrationEvent, IIntegrationEventHandler<ProductModelPriceUpdatedIntegrationEvent>>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthorization();
app.MapControllers();
app.Run();
