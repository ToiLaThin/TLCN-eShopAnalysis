using AutoMapper;
using eShopAnalysis.StockInventory.Data;
using eShopAnalysis.StockInventory.Repository;
using eShopAnalysis.StockInventory.Utilities;
using eShopAnalysis.StockInventoryAPI.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
