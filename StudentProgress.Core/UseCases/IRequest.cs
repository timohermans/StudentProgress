using MediatR;

namespace StudentProgress.Core.UseCases;

public interface IUseCaseRequest : IRequest { }
 
public interface IUseCaseRequest<IResult> : IRequest<IResult>
{
}
