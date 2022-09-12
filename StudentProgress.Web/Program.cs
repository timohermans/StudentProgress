using System;
using System.Net.Http;
using HtmlTags;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StudentProgress.Core;
using StudentProgress.Core.CanvasApi;
using StudentProgress.Core.Entities;
using StudentProgress.Web;
using StudentProgress.Web.Configuration;
using StudentProgress.Web.Infrastructure;

// TODO: Make sure you cannot import with no settings key
// TODO: Display Student Avatar
// TODO: Fix and test upsert import avatar Upsert
// TODO: Add Feedpulse reminders

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMiniProfiler().AddEntityFramework();
builder.Services.AddRazorPages();
builder.Services.AddHtmlTags(new TagConventions());
builder.Services.AddDbContext<ProgressContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ProgressContext"),
        b => b.MigrationsAssembly("StudentProgress.Core")));

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

app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();

using var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
var context = serviceScope.ServiceProvider.GetService<ProgressContext>();
context?.Database.Migrate();