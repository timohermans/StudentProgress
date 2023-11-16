using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace StudentProgress.Web.Lib.Configuration;

public class CoreConfiguration(IConfiguration config, IWebHostEnvironment environment) : ICoreConfiguration
{
    public string MediaLocation
    {
        get
        {
            var path = config.GetValue<string>("Media:Path");

            if (path == null) throw new ArgumentException(nameof(path));

            if (environment.IsDevelopment())
            {
                path = Path.Combine(AppContext.BaseDirectory, path);
            }

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            return path;
        }
    }
}