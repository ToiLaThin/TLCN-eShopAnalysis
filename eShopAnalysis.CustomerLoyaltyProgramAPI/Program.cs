using AutoMapper;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Data;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Repository;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Service;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Utilities.Behaviors;
using eShopAnalysis.EventBus.Abstraction;
using eShopAnalysis.EventBus.Extension;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEventBus(builder.Configuration);
//builder.Services.AddTransient<IIntegrationEventHandler<UserAppliedCouponToCartIntegrationEvent>, UserAppliedCouponToCartIntegrationEventHandling>();

builder.Host.UseSerilog((context, config) => {
    config.ReadFrom.Configuration(context.Configuration);
});
builder.Services.AddScoped<LoggingBehaviorActionFilter>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<PostgresDbContext>(ctxOptions =>
{
    ctxOptions.UseNpgsql(builder.Configuration.GetConnectionString("CustomerLoyaltyProgramConnString"), op => op.MigrationsHistoryTable("__EFMigrationsHistory", "CustomerLoyaltyProgram"))
              .LogTo(Console.WriteLine, LogLevel.Information); //how to log db command from serilog
});

builder.Services.AddScoped<IUserRewardPointRepository, UserRewardPointRepository>();
builder.Services.AddScoped<IRewardTransactionRepository, RewardTransactionRepository>();
builder.Services.AddScoped<IRewardTransactionService, RewardTransactionService>();
builder.Services.AddScoped<IUserRewardPointService, UserRewardPointService>();
//config automapper
var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddMaps(assembliesToScan: Assembly.GetExecutingAssembly());
});
IMapper mapper = new Mapper(mapperConfig);
builder.Services.AddSingleton(mapper);

var app = builder.Build();
app.UseSerilogRequestLogging();
var eventBus = app.Services.GetRequiredService<IEventBus>();
//eventBus.Subscribe<UserAppliedCouponToCartIntegrationEvent, IIntegrationEventHandler<UserAppliedCouponToCartIntegrationEvent>>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthorization();
app.MapControllers();
app.Run();
