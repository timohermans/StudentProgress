using MediatR;
using StudentProgress.Core.UseCases;

namespace StudentProgress.Api;

public static class MediatrExtensions
{
    public static WebApplication MediateGet<TRequest>(this WebApplication app, string template) where TRequest : IUseCaseRequest<TRequest>
    {
        app.MapGet(template, async (IMediator mediator, [AsParameters] TRequest request) => await mediator.Send(request));
        return app;
    }
}
