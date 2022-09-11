using HtmlTags;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StudentProgress.Core.Entities;
using StudentProgress.Web.Infrastructure;

// TODO: Integrate Canvas API
// TODO: upsert avatar
// TODO: Add Feedpulse reminders

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMiniProfiler().AddEntityFramework();
builder.Services.AddRazorPages();
builder.Services.AddHtmlTags(new TagConventions());
builder.Services.AddDbContext<ProgressContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ProgressContext"),
        b => b.MigrationsAssembly("StudentProgress.Core")));

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