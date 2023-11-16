using Microsoft.Extensions.Configuration;

namespace StudentProgress.Core.CanvasApi;

public class CanvasConfiguration(IConfiguration config) : ICanvasApiConfig
{
    public string? CanvasApiKey => config.GetValue<string>("Canvas:ApiKey");
    public string? CanvasApiUrl => config.GetValue<string>("Canvas:ApiUrl");

    public bool CanUseCanvasApiAsync() => CanvasApiKey != null && CanvasApiUrl != null;
}