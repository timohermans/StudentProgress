using Microsoft.Extensions.Configuration;
using StudentProgress.Web.Lib.CanvasApi;

namespace StudentProgress.Web.Lib.Configuration;

public class CanvasConfiguration(IConfiguration config) : ICanvasApiConfig
{
    public string? CanvasApiKey => config.GetValue<string>("Canvas:ApiKey");
    public string? CanvasApiUrl => config.GetValue<string>("Canvas:ApiUrl");

    public bool CanUseCanvasApiAsync() => CanvasApiKey != null && CanvasApiUrl != null;
}