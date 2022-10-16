using MediatR;

namespace StudentProgress.Core.UseCases;

public class EmptyCommand<TResponse> : IRequest<TResponse> { }

public interface IUseCaseBase<TResponse> : IRequestHandler<EmptyCommand<TResponse>, TResponse> {
}

public interface IUseCaseBase<in TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
}