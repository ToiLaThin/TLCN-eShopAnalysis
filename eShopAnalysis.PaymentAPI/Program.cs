using eShopAnalysis.PaymentAPI.Data;
using eShopAnalysis.PaymentAPI.Repository;
using eShopAnalysis.PaymentAPI.Service;
using eShopAnalysis.PaymentAPI.Service.Strategy;
using eShopAnalysis.PaymentAPI.UnitOfWork;
using eShopAnalysis.PaymentAPI.Utilities;
using Microsoft.EntityFrameworkCore;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

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
    return new PaymentService<MomoPaymentStrategy>(unitOfWork, momoPaymentStrategy);
});
builder.Services.AddScoped<IPaymentService<StripePaymentStrategy>>(sp =>
{
    //MomoPaymentStrategy is implementaton of IPaymentStrategy
    IUnitOfWork unitOfWork = sp.GetService<IUnitOfWork>();
    StripePaymentStrategy stripePaymentStrategy = sp.GetService<StripePaymentStrategy>();
    return new PaymentService<StripePaymentStrategy>(unitOfWork, stripePaymentStrategy);
});

builder.Services.AddScoped<IUserCustomerMappingRepository, UserCustomerMappingRepository>();

builder.Services.AddDbContext<PaymentContext>(ctxOption =>
{
    ctxOption.UseSqlServer(builder.Configuration.GetConnectionString("PaymentDb"));
});
StripeConfiguration.ApiKey = "sk_test_51MKzuQK5g3RpaRBrDoRZF32WRPWdGWDF5uUsJNX8hLl7ruXj2hA5B23UXlhCEPMnqJ2k75Ah4Zl1Aw3niu2SRfdV00E9hnzp67";
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
