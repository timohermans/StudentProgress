using System;
using System.Net.Http;
using HtmlTags;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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
builder.Services.AddMiniProfiler().AddEntityFramework();
builder.Services.AddRazorPages();
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


var app = builder.Build();

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

app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();