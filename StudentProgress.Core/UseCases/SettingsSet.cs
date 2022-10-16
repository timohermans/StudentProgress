using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;
using System.Threading;

namespace StudentProgress.Core.UseCases;

public class SettingsSet : IUseCaseBase<SettingsSet.Request, Result>
{
    private readonly ProgressContext _db;
    public class Request : IUseCaseRequest<Result>
    {
        public string CanvasApiKey { get; set; } = null!;
        public string CanvasApiUrl { get; set; } = null!;
    }

    public SettingsSet(ProgressContext db) => _db = db;

    public async Task<Result> Handle(Request request, CancellationToken token)
    {
        var upsertKeyTask = UpsertSetting(Setting.Keys.CanvasApiKey, request.CanvasApiKey, token);
        var upsertUrlTask = UpsertSetting(Setting.Keys.CanvasApiUrl, request.CanvasApiUrl, token);

        await Task.WhenAll(upsertKeyTask, upsertUrlTask);
        await _db.SaveChangesAsync(token);
        return Result.Success();
    }

    private async Task UpsertSetting(string key, string value, CancellationToken token)
    {
        var setting = await _db.Settings.FirstOrDefaultAsync(s => s.Key == key, token);

        if (setting == null)
        {
            setting = new Setting(key, value);
            await _db.Settings.AddAsync(setting, token);
        }
        else
        {
            setting.Update(value);
        }
    }
}