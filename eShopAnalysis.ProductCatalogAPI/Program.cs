using AutoMapper;
using eShopAnalysis.IdentityServer.Utilities;
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
builder.Services.AddScoped<MongoDbContext>();


builder.Services.AddScoped<ICatalogService, CatalogService>();
builder.Services.AddScoped<ICatalogRepository, CatalogRepository>();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddScoped<IDomainEventFactory, DomainEventFactory>();
builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

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
                    jwtOption.Authority = "https://localhost:7002";
                    jwtOption.SaveToken = true;
                    jwtOption.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = false,
                        ValidateActor = false,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    };
                });
builder.Services.AddAuthorization(authOption =>
{
    authOption.AddPolicy(PolicyNames.AdminPolicy, policy =>
    {
        policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
        policy.RequireRole("admin");
    });
    authOption.AddPolicy(PolicyNames.AuthenticatedUserPolicy, policy =>
    {
        policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
        policy.RequireRole("authenticatedUser");
    });
});

var app = builder.Build();

app.UseHttpLogging(); //middleware for logging request and resp https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-logging/?view=aspnetcore-7.0

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
