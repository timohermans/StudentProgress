using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using StudentProgress.Core.Entities;

namespace StudentProgress.Core.UseCases;

public class SettingsSet : UseCaseBase<SettingsSet.Request, Result>
{
    private readonly ProgressContext _db;
    public class Request
    {
        public string CanvasApiKey { get; set; } = null!;
    }

    public SettingsSet(ProgressContext db) => _db = db;

    public async Task<Result> HandleAsync(Request request)
    {
        var setting = _db.Settings.FirstOrDefault(s => s.Key == Setting.Keys.CanvasApiKey);

        if (setting == null)
        {
            setting = new Setting(Setting.Keys.CanvasApiKey, request.CanvasApiKey);
            await _db.Settings.AddAsync(setting);
        }
        else
        {
            setting.Update(request.CanvasApiKey);
        }
        
        await _db.SaveChangesAsync();
        return Result.Success();
    }
}