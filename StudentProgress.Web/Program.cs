using System.Net.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using StudentProgress.Core.CanvasApi;
using StudentProgress.Core.Constants;
using StudentProgress.Core.Data;
using StudentProgress.Core.Infrastructure;
using StudentProgress.Web.Lib.Configuration;
using CanvasClient = StudentProgress.Core.CanvasApi.CanvasClient;
using ICanvasApiConfig = StudentProgress.Core.CanvasApi.ICanvasApiConfig;
using ICanvasClient = StudentProgress.Core.CanvasApi.ICanvasClient;

var builder = WebApplication.CreateBuilder(args);

// Translations
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    string[] supportedCultures = ["en", "nl"];
    options.SetDefaultCulture(supportedCultures[0])
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
});

// HTMX specific
builder.Services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

// Authentication settings
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(AuthConstants.TwoFactorLoginPolicy, policy => policy.RequireClaim(AuthConstants.TwoFactorLoginPolicy, "2fa"));

// Razor Pages settings
builder.Services.AddRazorPages(options =>
    {
        options.Conventions.AuthorizeFolder("/");
        options.Conventions.AllowAnonymousToFolder("/Account");
    }).AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

// Session (For now, only use when passing around values too much (e.g. PersonId)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.Name = "ProgressSession";
    options.Cookie.HttpOnly = true;
});

// Database
builder.Services.AddDbContext<WebContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("WebContext"),
        b => b.MigrationsAssembly(typeof(Program).Assembly.FullName)));

// Inversion of Control
builder.Services.AddSingleton<IDateProvider, DateProvider>();
builder.Services.AddSingleton<ICoreConfiguration, CoreConfiguration>();
builder.Services.AddSingleton(_ =>
    new HttpClient(new SocketsHttpHandler { PooledConnectionIdleTimeout = TimeSpan.FromHours(1) }));
builder.Services.AddScoped<ICanvasApiConfig, CanvasConfiguration>();
builder.Services.AddScoped<ICanvasClient, CanvasClient>();

// Proxy settings (Running behind Traefik)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

var app = builder.Build();

app.UseForwardedHeaders();

app.UseRequestLocalization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.UseMiniProfiler();
    app.UseDeveloperExceptionPage();
}
else
{
    app.Use((context, next) =>
    {
        context.Request.Scheme = "https";
        return next();
    });
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseSession();

var mediaPath = app.Services.GetService<ICoreConfiguration>()!.MediaLocation;
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(mediaPath),
    RequestPath = new PathString("/media")
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();
public partial class Program { } // used for integration testing
