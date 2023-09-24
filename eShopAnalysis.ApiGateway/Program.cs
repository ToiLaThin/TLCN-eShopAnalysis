
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("ocelot.json", optional: true, reloadOnChange: true).Build();
string corPolicyName = "ocelotCorPolicy";

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
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) { }
app.UseCors(corPolicyName);
//app.UseHttpsRedirection();
app.UseOcelot();
app.UseAuthorization();

app.MapControllers();

app.Run();
