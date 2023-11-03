using AutoMapper;
using eShopAnalysis.EventBus.Abstraction;
using eShopAnalysis.EventBus.Extension;
using eShopAnalysis.StockInventory.Data;
using eShopAnalysis.StockInventory.Repository;
using eShopAnalysis.StockInventory.Utilities;
using eShopAnalysis.StockInventoryAPI.Services;
using eShopAnalysis.StockInventoryAPI.Utilities.Behaviors;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
//this will config all required by event bus, review appsettings.json EventBus section and EventBus Connection string
//new just subscribe integration event and integration event handler
builder.Services.AddEventBus(builder.Configuration);
//builder.Services.AddTransient<IIntegrationEventHandler<GracePeriodConfirmedIntegrationEvent>, GracePeriodConfirmedIntegrationEventHandler>();
builder.Host.UseSerilog((context, config) => {
    config.ReadFrom.Configuration(context.Configuration);
});
builder.Services.AddScoped<LoggingBehaviorActionFilter>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection(nameof(RedisSettings)));
builder.Services.AddSingleton<RedisDbContext>();
builder.Services.AddScoped<IStockInventoryRepository, StockInventoryRepository>();
builder.Services.AddScoped<IStockInventoryService, StockInventoryService>();

var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddMaps(assembliesToScan: Assembly.GetExecutingAssembly());
});
IMapper mapper = new Mapper(mapperConfig);
builder.Services.AddSingleton(mapper);

var app = builder.Build();
var eventBus = app.Services.GetRequiredService<IEventBus>();
//eventBus.Subscribe<UserCheckoutAcceptedIntegrationEvent, IIntegrationEventHandler<UserCheckoutAcceptedIntegrationEvent>>();
app.UseSerilogRequestLogging();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
