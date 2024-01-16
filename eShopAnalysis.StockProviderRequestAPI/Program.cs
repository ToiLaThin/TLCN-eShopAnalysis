using Serilog;
using eShopAnalysis.EventBus.Extension;
using eShopAnalysis.StockProviderRequestAPI.Utilities;
using eShopAnalysis.StockProviderRequestAPI.Data;
using AutoMapper;
using System.Reflection;
using eShopAnalysis.StockProviderRequestAPI.Utilities.Behaviors;
using eShopAnalysis.StockProviderRequestAPI.Repository;
using eShopAnalysis.StockProviderRequestAPI.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEventBus(builder.Configuration);
builder.Host.UseSerilog((context, config) => {
    config.ReadFrom.Configuration(context.Configuration);
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection(nameof(MongoDbSettings)));
builder.Services.AddScoped<MongoDbContext>();

builder.Services.AddScoped<IProviderRequirementRepository, ProviderRequirementRepository>();
builder.Services.AddScoped<IStockRequestTransactionRepository, StockRequestTransactionRepository>();
builder.Services.AddScoped<IProviderRequirementService, ProviderRequirementService>();
builder.Services.AddScoped<IStockRequestTransactionService, StockRequestTransactionService>();

var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddMaps(assembliesToScan: Assembly.GetExecutingAssembly());
});
IMapper mapper = new Mapper(mapperConfig);
builder.Services.AddSingleton(mapper);
builder.Services.AddScoped<LoggingBehaviorActionFilter>();
var app = builder.Build();
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
