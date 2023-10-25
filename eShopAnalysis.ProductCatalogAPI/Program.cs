using AutoMapper;
using eShopAnalysis.EventBus.Abstraction;
using eShopAnalysis.EventBus.Extension;
using eShopAnalysis.IdentityServer.Utilities;
using eShopAnalysis.ProductCatalogAPI.Application.BackChannelDto;
using eShopAnalysis.ProductCatalogAPI.Application.BackchannelServices;
using eShopAnalysis.ProductCatalogAPI.Application.Services;
using eShopAnalysis.ProductCatalogAPI.Domain.Models;
using eShopAnalysis.ProductCatalogAPI.Domain.SeedWork.FactoryMethod;
using eShopAnalysis.ProductCatalogAPI.Domain.SeedWork.Mediator;
using eShopAnalysis.ProductCatalogAPI.Infrastructure;
using eShopAnalysis.ProductCatalogAPI.Infrastructure.Data;
using eShopAnalysis.ProductCatalogAPI.Utilities;
using eShopAnalysis.ProductCatalogAPI.Utilities.Behaviors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
//this will config all required by event bus, review appsettings.json EventBus section and EventBus Connection string
//new just subscribe integration event and integration event handler
builder.Services.AddEventBus(builder.Configuration);
//builder.Services.AddTransient<IIntegrationEventHandler<GracePeriodConfirmedIntegrationEvent>, GracePeriodConfirmedIntegrationEventHandler>();

// Add services to the container.
builder.Services.AddControllers();
//config http logging for logging request
builder.Services.AddHttpLogging(configLog =>
{
    //configLog.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponseBody;
    configLog.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection(nameof(MongoDbSettings)));
builder.Services.Configure<BackChannelCommunication>(builder.Configuration.GetSection(nameof(BackChannelCommunication)));
builder.Services.AddScoped<MongoDbContext>();


builder.Services.AddScoped<ICatalogService, CatalogService>();
builder.Services.AddScoped<ICatalogRepository, CatalogRepository>();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddScoped<IBackChannelStockInventoryService, BackChannelStockInventoryService>();

builder.Services.AddScoped<IDomainEventFactory, DomainEventFactory>();
builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

builder.Services.AddHttpClient(); //resolve IHttpClientFactory
builder.Services.AddScoped<IBackChannelBaseService<StockInventoryDto, StockInventoryDto>, BackChannelBaseService<StockInventoryDto, StockInventoryDto>>();

//config automapper
var mapperConfig = new MapperConfiguration(cfg => 
{
    cfg.AddMaps(assembliesToScan: Assembly.GetExecutingAssembly());
});
IMapper mapper = new Mapper(mapperConfig);
builder.Services.AddSingleton(mapper);


builder.Services.AddScoped<LoggingBehaviorActionFilter>();

builder.Services.AddAuthentication()
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwtOption =>
                {
                    jwtOption.Authority = builder.Configuration.GetSection("OpenIdConnectAuthority:IdentityServerBaseUri").Value.ToString();
                    jwtOption.SaveToken = true;
                    jwtOption.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = false,
                        ValidateActor = false,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    };
                    jwtOption.RequireHttpsMetadata = false;
                });
builder.Services.AddAuthorization(authOption =>
{
    authOption.AddPolicy(PolicyNames.AdminPolicy, policy =>
    {
        policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
        policy.RequireRole(RoleType.Admin);
    });
    authOption.AddPolicy(PolicyNames.AuthenticatedUserPolicy, policy =>
    {
        policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
        policy.RequireRole(RoleType.AuthenticatedUser);
    });
});

var app = builder.Build();
var eventBus = app.Services.GetRequiredService<IEventBus>();
//eventBus.Subscribe<UserCheckoutAcceptedIntegrationEvent, IIntegrationEventHandler<UserCheckoutAcceptedIntegrationEvent>>();
app.UseHttpLogging(); //middleware for logging request and resp https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-logging/?view=aspnetcore-7.0

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
