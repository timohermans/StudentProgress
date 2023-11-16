using System.Threading;

namespace StudentProgress.Web.Lib.CanvasApi;

public interface ICanvasClient
{
    Task<CanvasResponse<T>?> GetAsync<T>(string query, CancellationToken token);
}
