using IdentityProvider.Constant;
using IdentityProvider.Context;
using IdentityProvider.Entity;
using IdentityProvider.Helper;
using IdentityProvider.Interface;
using IdentityProvider.Options;
using IdentityProvider.Repo;
using IdentityProvider.Seed;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var Services = builder.Services;
// My services
Services.AddScoped<IEmailSender, EmailSender>();
Services.AddScoped<ISmsSender, SmsSender>();
Services.Configure<EmailerOptions>(options => builder.Configuration.GetSection("EmailSettings").Bind(options));
Services.Configure<ReturnUrlOptions>(options => builder.Configuration.GetSection("ReturnUrl").Bind(options));

// Add services to the container.
Services.AddRazorPages();


// Keys
var path = Path.Combine(builder.Environment.ContentRootPath, "./Key");
Services.AddSingleton<DevKeys>();
Services.AddDataProtection()
        .SetApplicationName("IDP")
        .PersistKeysToFileSystem(new DirectoryInfo(path))
        .SetDefaultKeyLifetime(TimeSpan.FromDays(14));

// Database
Services.AddDbContext<ApplicationDbContext>(o =>
{
    o.UseMySql(
        builder.Configuration.GetConnectionString("OAuthServer"),
        ServerVersion.Create(new Version(10, 7, 8), ServerType.MariaDb),
        x => x.EnableRetryOnFailure()
        );
    o.UseOpenIddict();
});

// identity
Services.AddIdentity<ApplicationUser, ApplicationRole>(o =>
{
    o.SignIn.RequireConfirmedAccount = true;
    // password config
    o.Password.RequireDigit = true;
    o.Password.RequireNonAlphanumeric = false;
    o.Password.RequiredLength = 6;
    o.Password.RequireLowercase = true;
    o.Password.RequireUppercase = true;
    // user config
    o.User.RequireUniqueEmail = true;
    o.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789._@";
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

Services.ConfigureApplicationCookie(opts =>
{   
    // paths
    opts.LoginPath = "/signin";
    opts.AccessDeniedPath = "";
    opts.ReturnUrlParameter = builder.Configuration["ReturnUrl:Default"]!.ToString();
    opts.SlidingExpiration =true;
    opts.ClaimsIssuer= "https://localhost:5005";
    // Cookie config
    opts.Cookie.Name = "SIP";
    opts.Cookie.SameSite = SameSiteMode.Strict;
    opts.Cookie.HttpOnly = true;
    opts.Cookie.IsEssential =true;
    // Validate options
    opts.Validate();
});

// External Authentications
Services.AddAuthentication()
    .AddGoogle(
        opts =>
        {
            opts.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
            opts.ClientSecret = builder.Configuration["Authentication:Google:Secret"]!;
            opts.SignInScheme = IdentityConstants.ExternalScheme;
        })
    .AddFacebook(opts =>
        {
            opts.ClientId = builder.Configuration["Authentication:Facebook:ClientId"]!;
            opts.ClientSecret = builder.Configuration["Authentication:Facebook:Secret"]!;
            opts.SignInScheme = IdentityConstants.ExternalScheme;
        });

Services.ConfigureExternalCookie(x => x.Cookie.Name = "EIP");

// Configure Cookies
Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
    options.OnAppendCookie = cookieContext =>
    {
        if (cookieContext.CookieOptions.SameSite == SameSiteMode.None)
            cookieContext.CookieOptions.SameSite = SameSiteMode.Unspecified;
    };
    options.OnDeleteCookie = cookieContext =>
    {
        if (cookieContext.CookieOptions.SameSite == SameSiteMode.None)
            cookieContext.CookieOptions.SameSite = SameSiteMode.Unspecified;
    };
});
Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "AIP";
});

// Auth Schema
Services.AddAuthorization();

//Configure OpenIddict
Services.AddOpenIddict(x =>
{
    // Config core
    x.AddCore(opts => opts.UseEntityFrameworkCore().UseDbContext<ApplicationDbContext>());
    x.AddServer(options =>
    {
        options.AllowClientCredentialsFlow();
        options.AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange();

        options
            .SetAuthorizationEndpointUris("/connect/authorize")
            .SetTokenEndpointUris("/connect/token");

            // Encryption and signing of tokens
        options
            .AddEphemeralEncryptionKey()
            .AddEphemeralSigningKey();

            // Register scopes (permissions)
        options.RegisterScopes("api");

            // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
        options
            .UseAspNetCore()
            .EnableTokenEndpointPassthrough()
            .EnableAuthorizationEndpointPassthrough();
            
            // Certificates
        options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate();
            // Issuer
        options.SetIssuer(new Uri("https://localhost:5005"));
    });
    x.AddClient(options=>{
        options.UseSystemNetHttp();
    });
    x.AddValidation(options=>{
        options.UseSystemNetHttp();
    });

});

// Only for Development Configurations
if (builder.Environment.EnvironmentName == "Development")
{
    Services.AddHostedService<DemoData>();
}

Services.AddHostedService<SeedRoles>();

var app = builder.Build();

app.UseStaticFiles();
app.UseCookiePolicy();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultControllerRoute();

app.MapRazorPages();

app.Run();
