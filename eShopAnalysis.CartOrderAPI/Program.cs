using eShopAnalysis.EventBus.Abstraction;
using eShopAnalysis.EventBus.Extension;
using eShopAnalysis.CartOrderAPI.Application.BackchannelDto;
using eShopAnalysis.CartOrderAPI.Infrastructure;
using eShopAnalysis.CartOrderAPI.Infrastructure.Repositories;
using eShopAnalysis.CartOrderAPI.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using eShopAnalysis.CartOrderAPI.Application.Queries;
using eShopAnalysis.CartOrderAPI.IntegrationEvents;
using eShopAnalysis.CartOrderAPI.Application.IntegrationEvents.EventHandling;
using Serilog;
using eShopAnalysis.CartOrderAPI.Utilities.Behaviors;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);
//this will config all required by event bus, review appsettings.json EventBus section and EventBus Connection string
//new just subscribe integration event and integration event handler
builder.Services.AddEventBus(builder.Configuration);
builder.Host.UseSerilog((context, config) => {
    config.ReadFrom.Configuration(context.Configuration);
});
builder.Services.AddScoped<LoggingBehaviorActionFilter>();
builder.Services.AddTransient<IIntegrationEventHandler<OrderPaymentTransactionCompletedIntegrationEvent>, OrderPaymentTransactionCompletedIntegrationEventHandling>();

builder.Services.AddControllers();
builder.Services.AddDbContext<OrderCartContext>(ctxOption =>
{
    ctxOption.UseSqlServer(builder.Configuration.GetConnectionString("OrderCartDb"))
             .LogTo(Console.WriteLine, LogLevel.Information); ;
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

//config dapper
builder.Services.AddScoped<IOrderQueries>(sp => 
    new OrderQueries(builder.Configuration.GetConnectionString("OrderCartDb"))
);

//config automapper
var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddMaps(assembliesToScan: Assembly.GetExecutingAssembly());
});
IMapper mapper = new Mapper(mapperConfig);
builder.Services.AddSingleton(mapper);

var app = builder.Build();
var eventBus = app.Services.GetRequiredService<IEventBus>();
eventBus.Subscribe<OrderPaymentTransactionCompletedIntegrationEvent, IIntegrationEventHandler<OrderPaymentTransactionCompletedIntegrationEvent>>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSerilogRequestLogging();
app.UseAuthorization();
app.MapControllers();
app.Run();
