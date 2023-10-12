using System.Threading;
using System.Threading.Tasks;

namespace StudentProgress.Web.Lib.CanvasApi;

public interface ICanvasClient
{
    Task<CanvasResponse<T>?> GetAsync<T>(string query, CancellationToken token);
}
