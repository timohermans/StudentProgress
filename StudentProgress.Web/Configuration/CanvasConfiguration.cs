using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.CanvasApi;
using StudentProgress.Core.Entities;

namespace StudentProgress.Web.Configuration;

public class CanvasConfiguration : ICanvasApiConfig
{
    private readonly ProgressContext _db;

    public CanvasConfiguration(ProgressContext db) => _db = db;

    // TODO: prevent EF from querying this all the time (talking about a hassle, as I can set this in settings :))
    public string? CanvasApiKey => _db.Settings.FirstOrDefault(s => s.Key == Setting.Keys.CanvasApiKey)?.Value;
    public string? CanvasApiUrl => _db.Settings.FirstOrDefault(s => s.Key == Setting.Keys.CanvasApiUrl)?.Value;

    public async Task<bool> CanUseCanvasApiAsync() => await _db
        .Settings
        .Where(s => s.Key == Setting.Keys.CanvasApiKey || s.Key == Setting.Keys.CanvasApiUrl)
        .CountAsync() == 2;
}