
using eShopAnalysis.ApiGateway.Services.BackchannelDto;
using eShopAnalysis.ApiGateway.Services.BackchannelServices;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("ocelot.json", optional: true, reloadOnChange: true).Build();
string corPolicyName = "ocelotCorPolicy";
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOcelot(configuration);
builder.Services.AddCors((setup) =>
{
    setup.AddPolicy(corPolicyName, (options) =>
    {
        //options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin(); this will cause cors error when request have with credentials
        //ocelot is federation gateway which take and return to angular to it is set not Auth.Redirect
        options.AllowAnyHeader()
               .AllowAnyMethod()
               .WithOrigins("http://localhost:4200") //will set AccessControlAllowOrigin header
               .AllowCredentials();
    });
});
builder.Services.AddScoped<IBackChannelCartOrderService, BackChannelCartOrderService>();
builder.Services.AddScoped<IBackChannelStockInventoryService, BackChannelStockInventoryService>();
builder.Services.AddScoped<IBackChannelBaseService<PagingOrderRequestDto, OrderItemsResponseDto>, 
                            BackChannelBaseService<PagingOrderRequestDto, OrderItemsResponseDto>>();
builder.Services.AddScoped<IBackChannelBaseService<IEnumerable<Guid>, IEnumerable<ItemStockResponseDto>>, 
                            BackChannelBaseService<IEnumerable<Guid>, IEnumerable<ItemStockResponseDto>>>();
builder.Services.AddHttpClient(); //resolve IHttpClientFactory
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(corPolicyName);
//app.UseHttpsRedirection(); //needed to redirect to another https url
app.UseOcelot();
app.UseAuthorization();

app.MapControllers();

app.Run();
