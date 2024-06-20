using App.Metrics;
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
builder.Services.AddScoped<IBackChannelCustomerLoyaltyProgramService, BackChannelCustomerLoyaltyProgramService>();
builder.Services.AddScoped<IBackChannelStockProviderRequestService, BackChannelStockProviderRequestService>();
builder.Services.AddScoped(typeof(IBackChannelBaseService<,>), typeof(BackChannelBaseService<,>));
builder.Services.AddHttpClient(); //resolve IHttpClientFactory

var metricsBuilder = new MetricsBuilder().Report
                                         .ToInfluxDb("http://localhost:8086", "HealthCheckDb")
                                         .OutputMetrics
                                         .AsPrometheusPlainText();
metricsBuilder.Configuration.Configure(b =>
{
    b.DefaultContextLabel = "Aggregator";
    b.Enabled = true;
    b.ReportingEnabled = true;
});
var metrics = metricsBuilder.Build();
builder.Services.AddMetrics(metrics);
builder.Services.AddMetricsTrackingMiddleware();
builder.Services.AddMetricsEndpoints(setUp =>
{
    setUp.MetricsEndpointEnabled = true;
    setUp.MetricsTextEndpointEnabled = true;
    setUp.EnvironmentInfoEndpointEnabled = true;
    //can navigate to localhost/7009/metrics or /metrics-text and see metrics registered☻
});

var app = builder.Build();
app.UseSerilogRequestLogging();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseMetricsAllMiddleware();
app.UseMetricsAllEndpoints();
app.UseAuthorization();

app.MapControllers();

app.Run();
