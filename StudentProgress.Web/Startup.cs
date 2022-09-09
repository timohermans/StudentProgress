using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using StudentProgress.Core.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using HtmlTags;
using Microsoft.AspNetCore.HttpOverrides;
using StudentProgress.Web.Infrastructure;

namespace StudentProgress.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            ConfigureServices(services, false);
        }

        public void ConfigureProductionServices(IServiceCollection services)
        {
            ConfigureServices(services, true);
        }

        private void ConfigureServices(IServiceCollection services, bool requireHttpsOnAuth)
        {
            var isAuthenticationEnabled = Configuration.GetValue<bool>("Authentication:IsEnabled");
            services.AddMiniProfiler().AddEntityFramework();
            services.AddRazorPages(options =>
            {
                if (isAuthenticationEnabled) options.Conventions.AuthorizeFolder("/");
            });
            services.AddHtmlTags(new TagConventions());
            services.AddDbContext<ProgressContext>(options =>
                options.UseSqlite(BuildConnectionString(),
                    b => b.MigrationsAssembly("StudentProgress.Core")));

            if (isAuthenticationEnabled)
            {
                JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
                services.AddAuthentication(options =>
                    {
                        // Store the session to cookies
                        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        // OpenId authentication
                        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                    })
                    .AddCookie("Cookies")
                    .AddOpenIdConnect(options =>
                    {
                        options.ClientId = Configuration.GetValue<string>("Authentication:ClientId");
                        options.ClientSecret = Configuration.GetValue<string>("Authentication:ClientSecret");
                        options.Authority = Configuration.GetValue<string>("Authentication:Authority");

                        options.RequireHttpsMetadata = requireHttpsOnAuth;

                        options.SaveTokens = true;
                        options.GetClaimsFromUserInfoEndpoint = true;
                        options.ResponseType = OpenIdConnectResponseType.IdToken;
                    });

                services.Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders =
                        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                });
            }
        }

        private string BuildConnectionString()
        {
            var connectionString = Configuration.GetConnectionString("ProgressContext");
            if (!string.IsNullOrWhiteSpace(connectionString)) return connectionString;

            var dbMap = new Dictionary<string, string>
                {
                    {"User ID", Configuration.GetValue<string>("DB_USERNAME")},
                    {"Password", Configuration.GetValue<string>("DB_PASSWORD")},
                    {"Host", Configuration.GetValue<string>("DB_HOST") ?? "localhost"},
                    {"Port", Configuration.GetValue<string>("DB_PORT") ?? "5432"},
                    {"Database", Configuration.GetValue<string>("DB_DATABASE") ?? "student-progress"}
                }.Where(m => m.Value != null)
                .Select(m => $"{m.Key}={m.Value}")
                .ToList();

            return string.Join(';', dbMap);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseMiniProfiler();
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                if (Configuration.GetValue<bool>("Authentication:IsEnabled"))
                {
                    app.Use((context, next) =>
                    {
                        context.Request.Scheme = "https";
                        return next();
                    });
                }

                app.UseCookiePolicy(new CookiePolicyOptions()
                {
                    // is used for new chrome cookiepolicy
                    // see https://stackoverflow.com/questions/50262561/correlation-failed-in-net-core-asp-net-identity-openid-connect/64874175#64874175
                    MinimumSameSitePolicy = SameSiteMode.Lax
                });

                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            if (Configuration.GetValue<bool>("Authentication:IsEnabled"))
            {
                app.UseAuthentication();
                app.UseAuthorization();
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });

            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var context = serviceScope.ServiceProvider.GetService<ProgressContext>();
            context?.Database.Migrate();
        }
    }
}