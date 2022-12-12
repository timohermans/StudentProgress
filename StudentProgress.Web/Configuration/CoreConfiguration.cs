using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using StudentProgress.Core;

namespace StudentProgress.Web.Configuration;

public class CoreConfiguration : ICoreConfiguration
{
    private readonly IConfiguration _config;
    private readonly IWebHostEnvironment _environment;

    public CoreConfiguration(IConfiguration config, IWebHostEnvironment environment)
    {
        _config = config;
        _environment = environment;
    }

    public string MediaLocation
    {
        get
        {
            var path = _config.GetValue<string>("Media:Path");

            if (path == null) throw new ArgumentException(nameof(path));

            if (_environment.IsDevelopment())
            {
                path = Path.Combine(AppContext.BaseDirectory, path);
            }

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            return path;
        }
    }
}