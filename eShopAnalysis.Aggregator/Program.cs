using eShopAnalysis.Aggregator.Services.BackchannelService;
using eShopAnalysis.Aggregator.Services.BackchannelServices;
using eShopAnalysis.Aggregator.Utilities;
using eShopAnalysis.Aggregator.Utilities.Behaviors;
using eShopAnalysis.CartOrderAPI.Application.BackchannelServices;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) => {
    config.ReadFrom.Configuration(context.Configuration);
});
builder.Services.AddScoped<LoggingBehaviorActionFilter>();
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
