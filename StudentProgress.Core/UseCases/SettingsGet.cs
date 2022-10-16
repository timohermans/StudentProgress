using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;
using System.Threading;

namespace StudentProgress.Core.UseCases;

public class SettingsGet : IUseCaseBase<SettingsGet.Request, SettingsGet.Response>
{
    private readonly ProgressContext _db;
    public record Request: IUseCaseRequest<Response>;
    public record Response(string? CanvasApiKey, string? CanvasUrl);

    public SettingsGet(ProgressContext db) => _db = db;

    public async Task<Response> Handle(Request _, CancellationToken token)
    {
        var settings =  await _db.Settings.ToListAsync(token);

        var settingsPerKey = settings.ToDictionary(k => k.Key, v => v.Value);
        return new Response(
            settingsPerKey.GetValueOrDefault(Setting.Keys.CanvasApiKey),
            settingsPerKey.GetValueOrDefault(Setting.Keys.CanvasApiUrl));
    }
}