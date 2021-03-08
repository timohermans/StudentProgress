using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using StudentProgress.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentProgress.Api
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
            services.AddDbContext<ProgressContext>(options =>
               options.UseNpgsql(BuildConnectionString(),
                   b => b.MigrationsAssembly("StudentProgress.Core")));
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "StudentProgress.Api", Version = "v1" });
            });
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
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "StudentProgress.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
