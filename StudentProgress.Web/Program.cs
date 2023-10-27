using System;
using System.Net.Http;
using HtmlTags;
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
using StudentProgress.Core.Entities;
using StudentProgress.Web.Lib.Configuration;
using StudentProgress.Web.Lib.Constants;
using StudentProgress.Web.Lib.Data;
using StudentProgress.Web.Lib.Infrastructure;
using CanvasClient = StudentProgress.Web.Lib.CanvasApi.CanvasClient;
using ICanvasApiConfig = StudentProgress.Web.Lib.CanvasApi.ICanvasApiConfig;
using ICanvasClient = StudentProgress.Web.Lib.CanvasApi.ICanvasClient;

var builder = WebApplication.CreateBuilder(args);


builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "en", "nl" };
    options.SetDefaultCulture(supportedCultures[0])
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
});
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthConstants.TwoFactorLoginPolicy, policy => policy.RequireClaim(AuthConstants.TwoFactorLoginPolicy, "2fa"));
});

builder.Services.AddMiniProfiler().AddEntityFramework();

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
builder.Services.AddHtmlTags(new TagConventions());
builder.Services.AddDbContext<WebContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("WebContext")));
builder.Services.AddDbContext<ProgressContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ProgressContext"),
        b => b.MigrationsAssembly("StudentProgress.Core")));

builder.Services.AddSingleton<IDateProvider, DateProvider>();
builder.Services.AddSingleton<ICoreConfiguration, CoreConfiguration>();
builder.Services.AddSingleton(_ =>
    new HttpClient(new SocketsHttpHandler { PooledConnectionIdleTimeout = TimeSpan.FromHours(1) }));
builder.Services.AddScoped<ICanvasApiConfig, CanvasConfiguration>();
builder.Services.AddScoped<ICanvasClient, CanvasClient>();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

var app = builder.Build();

app.UseForwardedHeaders();

app.UseRequestLocalization();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.Use((context, next) =>
    {
        context.Request.Scheme = "https";
        return next();
    });
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseMiniProfiler();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
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