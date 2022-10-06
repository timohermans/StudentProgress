using System;
using System.Net.Http;
using Auth0.AspNetCore.Authentication;
using HtmlTags;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using StudentProgress.Core;
using StudentProgress.Core.CanvasApi;
using StudentProgress.Core.Entities;
using StudentProgress.Web;
using StudentProgress.Web.Configuration;
using StudentProgress.Web.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.ConfigureApplicationCookie(options => { options.Cookie.SameSite = SameSiteMode.None; });

builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth0:Domain"];
    options.ClientId = builder.Configuration["Auth0:ClientId"];
});

builder.Services.AddMiniProfiler().AddEntityFramework();

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Canvas");
    options.Conventions.AuthorizeFolder("/Milestones");
    options.Conventions.AuthorizeFolder("/Progress");
    options.Conventions.AuthorizeFolder("/Settings");
    options.Conventions.AuthorizeFolder("/StudentGroups");
});
builder.Services.AddHtmlTags(new TagConventions());
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

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseMiniProfiler();
    app.UseBrowserLink();
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