using IdentityProvider.Context;
using IdentityProvider.Endpoints.OAuth;
using IdentityProvider.Entity;
using IdentityProvider.Helper;
using IdentityProvider.Interface;
using IdentityProvider.Options;
using IdentityProvider.Repo;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

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

var app = builder.Build();

app.UseStaticFiles();
app.UseCookiePolicy();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/auth/authorize", Authorize.Handle).RequireAuthorization();
app.MapPost("/auth/token", Token.Handle);

app.MapRazorPages();

app.Run();
