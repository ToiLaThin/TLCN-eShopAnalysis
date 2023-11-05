using eShopAnalysis.PaymentAPI.Data;
using eShopAnalysis.PaymentAPI.Repository;
using eShopAnalysis.PaymentAPI.Service;
using eShopAnalysis.PaymentAPI.Service.Strategy;
using eShopAnalysis.PaymentAPI.UnitOfWork;
using eShopAnalysis.PaymentAPI.Utilities;
using Microsoft.EntityFrameworkCore;
using eShopAnalysis.EventBus.Extension;
using Stripe;
using eShopAnalysis.EventBus.Abstraction;
using Serilog;
using eShopAnalysis.PaymentAPI.Utilities.Behaviors;

var builder = WebApplication.CreateBuilder(args);

//this will config all required by event bus, review appsettings.json EventBus section and EventBus Connection string
//new just subscribe integration event and integration event handler
builder.Services.AddEventBus(builder.Configuration);
//builder.Services.AddTransient<IIntegrationEventHandler<GracePeriodConfirmedIntegrationEvent>, GracePeriodConfirmedIntegrationEventHandler>();
builder.Host.UseSerilog((context, config) => {
    config.ReadFrom.Configuration(context.Configuration);
});
builder.Services.AddScoped<LoggingBehaviorActionFilter>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
//builder.Services.AddScoped<IPaymentService<MomoPaymentStrategy>, PaymentService<MomoPaymentStrategy>>(); 
//this will have error since it does not know how to init PaymentService<MomoPaymentStrategy>
//the constructor have IPaymentStrategy which have multiple implementation ,so it cannot resolve which to pass in in that constructor

//to explicit specify how PaymentService<MomoPaymentStrategy> we must specify the constructor and the type we pass in
builder.Services.Configure<MomoSetting>(builder.Configuration.GetSection(nameof(MomoSetting)));
builder.Services.Configure<StripeSetting>(builder.Configuration.GetSection(nameof(StripeSetting)));
builder.Services.AddScoped<MomoPaymentStrategy>();
builder.Services.AddScoped<StripePaymentStrategy>();
builder.Services.AddScoped<IPaymentService<MomoPaymentStrategy>>(sp =>
{
    //MomoPaymentStrategy is implementaton of IPaymentStrategy
    IUnitOfWork unitOfWork = sp.GetService<IUnitOfWork>();
    MomoPaymentStrategy momoPaymentStrategy = sp.GetService<MomoPaymentStrategy>(); 
    IEventBus eventBus = sp.GetService<IEventBus>();
    return new PaymentService<MomoPaymentStrategy>(unitOfWork, momoPaymentStrategy, eventBus);
});
builder.Services.AddScoped<IPaymentService<StripePaymentStrategy>>(sp =>
{
    //MomoPaymentStrategy is implementaton of IPaymentStrategy
    IUnitOfWork unitOfWork = sp.GetService<IUnitOfWork>();
    StripePaymentStrategy stripePaymentStrategy = sp.GetService<StripePaymentStrategy>();
    IEventBus eventBus = sp.GetService<IEventBus>();
    return new PaymentService<StripePaymentStrategy>(unitOfWork, stripePaymentStrategy, eventBus);
});

builder.Services.AddScoped<IUserCustomerMappingRepository, UserCustomerMappingRepository>();

builder.Services.AddDbContext<PaymentContext>(ctxOption =>
{
    ctxOption.UseSqlServer(builder.Configuration.GetConnectionString("PaymentDb"))
             .LogTo(Console.WriteLine, LogLevel.Information); ;
});
StripeConfiguration.ApiKey = "sk_test_51MKzuQK5g3RpaRBrDoRZF32WRPWdGWDF5uUsJNX8hLl7ruXj2hA5B23UXlhCEPMnqJ2k75Ah4Zl1Aw3niu2SRfdV00E9hnzp67";
var app = builder.Build();
var eventBus = app.Services.GetRequiredService<IEventBus>();
//eventBus.Subscribe<UserCheckoutAcceptedIntegrationEvent, IIntegrationEventHandler<UserCheckoutAcceptedIntegrationEvent>>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSerilogRequestLogging();
app.UseHttpsRedirection(); //for redirect to stripe? but now we do not redirect , do we still need this

app.UseAuthorization();

app.MapControllers();

app.Run();
