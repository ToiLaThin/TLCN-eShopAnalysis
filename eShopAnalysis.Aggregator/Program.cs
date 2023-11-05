using eShopAnalysis.Aggregator.Services.BackchannelService;
using eShopAnalysis.Aggregator.Services.BackchannelServices;
using eShopAnalysis.Aggregator.Utilities;
using eShopAnalysis.CartOrderAPI.Application.BackchannelServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<BackChannelCommunication>(builder.Configuration.GetSection(nameof(BackChannelCommunication)));
builder.Services.AddScoped<IBackChannelCartOrderService, BackChannelCartOrderService>();
builder.Services.AddScoped<IBackChannelStockInventoryService, BackChannelStockInventoryService>();
builder.Services.AddScoped<IBackChannelCouponSaleItemService, BackChannelCouponSaleItemService>();
builder.Services.AddScoped<IBackChannelProductCatalogService, BackChannelProductCatalogService>();
builder.Services.AddScoped(typeof(IBackChannelBaseService<,>), typeof(BackChannelBaseService<,>));
builder.Services.AddHttpClient(); //resolve IHttpClientFactory
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
