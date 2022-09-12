using CSharpFunctionalExtensions;
using StudentProgress.Core.Entities;

namespace StudentProgress.Core.UseCases;

public class SettingsSet : UseCaseBase<SettingsSet.Request, Result>
{
    private readonly ProgressContext _db;
    public class Request
    {
        public string CanvasApiKey { get; set; } = null!;
        public string CanvasApiUrl { get; set; } = null!;
    }

    public SettingsSet(ProgressContext db) => _db = db;

    public async Task<Result> HandleAsync(Request request)
    {
        var upsertKeyTask = UpsertSetting(Setting.Keys.CanvasApiKey, request.CanvasApiKey);
        var upsertUrlTask = UpsertSetting(Setting.Keys.CanvasApiUrl, request.CanvasApiUrl);

        await Task.WhenAll(upsertKeyTask, upsertUrlTask);
        await _db.SaveChangesAsync();
        return Result.Success();
    }

    private async Task UpsertSetting(string key, string value)
    {
        var setting = _db.Settings.FirstOrDefault(s => s.Key == key);

        if (setting == null)
        {
            setting = new Setting(key, value);
            await _db.Settings.AddAsync(setting);
        }
        else
        {
            setting.Update(value);
        }
    }
}