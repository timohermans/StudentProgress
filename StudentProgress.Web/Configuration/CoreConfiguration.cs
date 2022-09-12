using Microsoft.Extensions.Configuration;
using StudentProgress.Core;

namespace StudentProgress.Web.Configuration;

public class CoreConfiguration : ICoreConfiguration
{
    private readonly IConfiguration _config;

    public CoreConfiguration(IConfiguration config) => _config = config;

    public string MediaLocation => _config.GetValue<string>("Media:Path");
}