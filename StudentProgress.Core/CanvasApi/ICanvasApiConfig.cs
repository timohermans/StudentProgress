namespace StudentProgress.Core.CanvasApi;

public interface ICanvasApiConfig
{
    string? CanvasApiKey { get; }
    string? CanvasApiUrl { get; }
}
