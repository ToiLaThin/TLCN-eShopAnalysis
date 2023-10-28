using eShopAnalysis.Aggregator.Services.BackchannelDto;
using eShopAnalysis.Aggregator.Services.BackchannelServices;
using eShopAnalysis.Aggregator.Utilities;
using eShopAnalysis.ApiGateway.Services.BackchannelServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<BackChannelCommunication>(builder.Configuration.GetSection(nameof(BackChannelCommunication)));
builder.Services.AddScoped<IBackChannelCartOrderService, BackChannelCartOrderService>();
builder.Services.AddScoped<IBackChannelStockInventoryService, BackChannelStockInventoryService>();
builder.Services.AddScoped<IBackChannelBaseService<PagingOrderRequestDto, IEnumerable<OrderItemsResponseDto>>,
                            BackChannelBaseService<PagingOrderRequestDto, IEnumerable<OrderItemsResponseDto>>>();
builder.Services.AddScoped<IBackChannelBaseService<OrderItemsStockRequestDto, IEnumerable<ItemStockResponseDto>>,
                            BackChannelBaseService<OrderItemsStockRequestDto, IEnumerable<ItemStockResponseDto>>>();
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
