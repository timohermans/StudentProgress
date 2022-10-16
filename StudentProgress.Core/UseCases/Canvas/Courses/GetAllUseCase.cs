using StudentProgress.Core.CanvasApi;
using StudentProgress.Core.CanvasApi.Models;
using System.Threading;

namespace StudentProgress.Core.UseCases.Canvas.Courses;

public class GetAllUseCase : IUseCaseBase<GetAllUseCase.EmptyCommand, GetAllUseCase.Response>
{
    public record EmptyCommand : IUseCaseRequest<Response>;
    public record Response(IEnumerable<Course> Courses);

    private readonly ICanvasClient _client;

    public GetAllUseCase(ICanvasClient client) => _client = client;

    public async Task<Response> Handle(EmptyCommand request, CancellationToken token)
    {
        var query = @"query MyQuery {
    allCourses {
        _id
        name
        term {
            _id
            name
            startAt
            endAt
        }
    }
}
";
        var data = await _client.GetAsync<ApiResponse>(query, token);
        return new Response(data?.Data?.AllCourses?.OrderByDescending(c => c.Term?.StartAt).ToList() ?? new List<Course>());
    }
    public class ApiResponse
    {
        public IEnumerable<Course>? AllCourses { get; set; }
    }
}
