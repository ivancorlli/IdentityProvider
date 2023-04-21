using IdentityProvider.Context;
using IdentityProvider.Endpoints.OAuth;
using IdentityProvider.Entity;
using IdentityProvider.Helper;
using IdentityProvider.Interface;
using IdentityProvider.Options;
using IdentityProvider.Repo;
using IdentityProvider.Seed;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using static OpenIddict.Abstractions.OpenIddictConstants;

var builder = WebApplication.CreateBuilder(args);

var Services = builder.Services;
// My services
Services.AddScoped<IEmailSender, EmailSender>();
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
Services.AddIdentity<ApplicationUser, IdentityRole>(o =>
{
    o.SignIn.RequireConfirmedAccount = true;

    o.User.RequireUniqueEmail = true;
    o.Password.RequireNonAlphanumeric = false;

})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

Services.ConfigureApplicationCookie(opts =>
{
    opts.Cookie.Name = "SIP";
    opts.LoginPath = "/signin";
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
    options.OnAppendCookie = cookieContext => {
        if (cookieContext.CookieOptions.SameSite == SameSiteMode.None)
            cookieContext.CookieOptions.SameSite = SameSiteMode.Unspecified;
    };
    options.OnDeleteCookie = cookieContext =>
    {
        if (cookieContext.CookieOptions.SameSite == SameSiteMode.None)
            cookieContext.CookieOptions.SameSite = SameSiteMode.Unspecified;
    };
});
Services.AddAntiforgery(options => {
    options.Cookie.Name = "AIP";
});

// Auth Schema
Services.AddAuthorization();

//Configure OpenIddict
Services.AddOpenIddict(x =>
{
    // Config core
    x.AddCore(opts=>opts.UseEntityFrameworkCore().UseDbContext<ApplicationDbContext>());
    // Config server
    x.AddServer(opts =>
    {
        opts.SetAuthorizationEndpointUris("/connect/authorize").SetTokenEndpointUris("/connect/token");
        opts.AllowClientCredentialsFlow().AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange();

        // Encryption and Signin of tokens 
        opts.AddEphemeralEncryptionKey();
        opts.AddEphemeralSigningKey();

        opts.SetIssuer(new Uri("https://localhost:5005"));

        // Scopes
        opts.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles, "Api");

        opts.UseAspNetCore()
            .EnableTokenEndpointPassthrough()
            .EnableAuthorizationEndpointPassthrough()
            .EnableStatusCodePagesIntegration();
	})
    .AddValidation(options =>
    {
        options.UseLocalServer();

        options.UseAspNetCore();
    });
});

// Only for Development Configurations
if(builder.Environment.EnvironmentName == "Development")
{
    Services.AddHostedService<TestData>();
}



var app = builder.Build();

app.UseStaticFiles();
app.UseCookiePolicy();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("~/connect/authorize", Authorize.HandleAsync).RequireAuthorization();
app.MapPost("~/connect/authorize", Authorize.HandleAsync).RequireAuthorization();
app.MapPost("~/connect/token", Token.Exchange);

app.MapRazorPages();

app.Run();
