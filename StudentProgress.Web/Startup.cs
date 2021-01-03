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
using Npgsql;

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
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMiniProfiler().AddEntityFramework();
            services.AddRazorPages(options => { options.Conventions.AuthorizeFolder("/"); });

            services.AddDbContext<ProgressContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("ProgressContext"),
                    b => b.MigrationsAssembly("StudentProgress.Core")));
            services.AddScoped<IDbConnection>(_ =>
                new NpgsqlConnection(Configuration.GetConnectionString("ProgressContext")));

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

                    // because we don't expose to the outside, we can use this in production as well
                    options.RequireHttpsMetadata = false;

                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.ResponseType = OpenIdConnectResponseType.IdToken;
                });
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
                app.UseCookiePolicy(new CookiePolicyOptions()
                {
                    // is used for new chrome cookiepolicy
                    // see https://stackoverflow.com/questions/50262561/correlation-failed-in-net-core-asp-net-identity-openid-connect/64874175#64874175
                    MinimumSameSitePolicy = SameSiteMode.Lax
                });

                // this is used for the reverse proxy
                app.UsePathBase("/student");
                app.Use((context, next) =>
                {
                    context.Request.PathBase = "/student";
                    return next();
                });

                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapRazorPages(); });
        }
    }
}