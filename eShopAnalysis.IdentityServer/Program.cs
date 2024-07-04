using eShopAnalysis.IdentityServer.Configuration;
using eShopAnalysis.IdentityServer.Controllers;
using eShopAnalysis.IdentityServer.Models;
using eShopAnalysis.IdentityServer.Utilities;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using eShopAnalysis.EventBus.Abstraction;
using eShopAnalysis.EventBus.Extension;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEventBus(builder.Configuration);
//config option pattern for email sender TODO refactor for better structure, extension maybe?
IConfiguration emailConfiguration = builder.Configuration.GetSection("MailOptions");
builder.Services.Configure<MailOptions>(emailConfiguration);
builder.Services.AddTransient<EmailSenderService, EmailSenderService>();

//config asp.net core identity for storing user infomation
string connString = builder.Configuration.GetConnectionString("AuthIdentityConnStr");
builder.Services.AddDbContext<EsaIdentityDbContext>(identityDbConfig =>
{
    identityDbConfig.UseSqlServer(connString, sqlServerConfig =>
    {
        sqlServerConfig.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
        sqlServerConfig.MigrationsHistoryTable("__EFMigrationHistory", "Identity");
    });
});
builder.Services.AddSingleton<ILogger, Logger<AuthController>>();
builder.Services.AddIdentity<EsaUser, IdentityRole>(identityConfig =>
{
    identityConfig.Password.RequiredLength = 4;
    identityConfig.Password.RequireDigit = false;
    identityConfig.Password.RequireUppercase = false;
    identityConfig.Password.RequireNonAlphanumeric = false;

    //config to require confirmed email
    identityConfig.SignIn.RequireConfirmedEmail = false;
})
    .AddEntityFrameworkStores<EsaIdentityDbContext>()
    .AddUserManager<UserManager<EsaUser>>()
    .AddSignInManager<SignInManager<EsaUser>>()
    .AddDefaultTokenProviders(); //for token confirm email generation
//configure cookie to store identity server session
builder.Services.ConfigureApplicationCookie(cookieConfig =>
{
    cookieConfig.Cookie.Name = "Identity.Cookie"; //=> cookie represent authenticated with idenityserver via google diff from Identity.External
    cookieConfig.LoginPath = "/Auth/Login";
    cookieConfig.LogoutPath = "/Auth/Logout";
    cookieConfig.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    //cookieConfig.Cookie.HttpOnly = false; //to delete this cookie on signin callback another option might be setting the lifespan
    //cookieConfig.Cookie.MaxAge = TimeSpan.FromSeconds(120); //Identity cookie will exist for 2 min
    //if this is set , then even call signOutAsync in _signInManager. Identity Cookie won't be deleted, only 2 min passed
});
builder.Services.ConfigureExternalCookie(externalCookieConfig =>
{
    //config Identity cookie if sign in by google, this should be secure
    externalCookieConfig.Cookie.Name = "Identity.External";
    externalCookieConfig.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddIdentityServer(identityServerOption =>
{
    //rconfig some of the cookie handler of ids4
    identityServerOption.UserInteraction.LoginUrl = "/Auth/Login";
    identityServerOption.Authentication.CheckSessionCookieSameSiteMode = SameSiteMode.Lax; //can only set its samesite mode to not being none
    identityServerOption.Authentication.CheckSessionCookieName = "idsrv.session";
})
    .AddAspNetIdentity<EsaUser>()
    .AddInMemoryClients(IdentityServerConfiguration.GetClients())
    .AddInMemoryApiResources(IdentityServerConfiguration.GetApis())
    .AddInMemoryIdentityResources(IdentityServerConfiguration.GetIdentities())
    .AddInMemoryApiScopes(IdentityServerConfiguration.GetScopes())
    .AddDeveloperSigningCredential();

builder.Services.AddAuthentication()
                //all of this to fix redirect after login, gg login, all require secure as the samesite = none
                //BUT this will not work since it already set in AddIdentityServer(), these cookie handler already been register
                //.AddCookie(IdentityServerConstants.DefaultCookieAuthenticationScheme,(idsrvCookieConfig) =>
                //{
                //    //this cookie handler already have but we will override it
                //    idsrvCookieConfig.Cookie.Name = "idsrv.session";
                //    idsrvCookieConfig.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                //})
                //.AddCookie(IdentityServerConstants.ExternalCookieAuthenticationScheme, (externalCookieConfig) =>
                //{
                //    externalCookieConfig.Cookie.Name = "Identity.External";
                //    externalCookieConfig.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                //})
                .AddGoogle("Google", googleOption =>
                {
                    googleOption.ClientId = builder.Configuration.GetSection("Authentication:Google:ClientId").Value;
                    googleOption.ClientSecret = builder.Configuration.GetSection("Authentication:Google:ClientSecret").Value;
                    googleOption.SignInScheme = IdentityConstants.ExternalScheme; //Identity.External will be default => cookie represent authenticated with google
                                                                                  //googleOption.SaveTokens = true; //để lấy access token trong callback uri
                    googleOption.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always; //to solve correlation error
                });

string corPolicyName = "thinhnd"; //add cors
builder.Services.AddCors((setup) =>
{
    setup.AddPolicy(corPolicyName, (options) =>
    {
        options.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin();
    });
});
builder.Services.AddControllersWithViews();

var app = builder.Build();
var eventBus = app.Services.GetRequiredService<IEventBus>();
/*Migration for Identity*/
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // add 10 seconds delay to ensure the db server is up to accept connections
        // this won't be needed in real world application
        //System.Threading.Thread.Sleep(30000);
        var context = services.GetRequiredService<EsaIdentityDbContext>();
        context.Database.EnsureCreated();

    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}
app.UseCors(corPolicyName);
app.UseIdentityServer();
app.MapDefaultControllerRoute();
app.Run();

