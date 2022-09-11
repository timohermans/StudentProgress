using StudentProgress.Core.CanvasApi;
using StudentProgress.Core.CanvasApi.Models;

namespace StudentProgress.Core.UseCases.Canvas.Courses;

public class GetAllUseCase : UseCaseBase<IEnumerable<Course>>
{
    private readonly ICanvasClient _client;

    public GetAllUseCase(ICanvasClient client) => _client = client;

    public async Task<IEnumerable<Course>> HandleAsync()
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
        var data = await _client.GetAsync<ApiResponse>(query);
        return data?.Data?.AllCourses?.OrderByDescending(c => c.Term?.StartAt).ToList() ?? new List<Course>();
    }
}

public class ApiResponse
{
    public IEnumerable<Course>? AllCourses { get; set; }
}