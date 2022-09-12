using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;

namespace StudentProgress.Core.UseCases;

public class SettingsGet : UseCaseBase<SettingsGet.Response>
{
    private readonly ProgressContext _db;
    public record Response(string? CanvasApiKey, string? CanvasUrl);

    public SettingsGet(ProgressContext db) => _db = db;

    public async Task<Response> HandleAsync()
    {
        var settings =  await _db.Settings.ToListAsync();

        var settingsPerKey = settings.ToDictionary(k => k.Key, v => v.Value);
        return new Response(
            settingsPerKey.GetValueOrDefault(Setting.Keys.CanvasApiKey),
            settingsPerKey.GetValueOrDefault(Setting.Keys.CanvasApiUrl));
    }
}