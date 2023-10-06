using eShopAnalysis.CouponSaleItemAPI.Data;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using eShopAnalysis.CouponSaleItemAPI.UnitOfWork;
using eShopAnalysis.CouponSaleItemAPI.Service;
using AutoMapper;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<PostgresDbContext>(ctxOption =>
{
    ctxOption.UseNpgsql(builder.Configuration.GetConnectionString("CouponSaleItemConnString"));
});
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<ISaleItemService, SaleItemService>();

//config automapper
var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddMaps(assembliesToScan: Assembly.GetExecutingAssembly());
});
IMapper mapper = new Mapper(mapperConfig);
builder.Services.AddSingleton(mapper);

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthorization();
app.MapControllers();
app.Run();
