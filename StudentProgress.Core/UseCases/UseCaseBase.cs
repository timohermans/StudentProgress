using System.Threading.Tasks;

namespace StudentProgress.Core.UseCases;

public interface UseCaseBase<TResponse>
{
    Task<TResponse> HandleAsync();
}

public interface UseCaseBase<in TRequest, TResponse>
{
    Task<TResponse> HandleAsync(TRequest request);
}