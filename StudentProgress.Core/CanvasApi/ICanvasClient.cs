using System.Threading;

namespace StudentProgress.Core.CanvasApi;

public interface ICanvasClient
{
    Task<CanvasResponse<T>?> GetAsync<T>(string query, CancellationToken token);
}
