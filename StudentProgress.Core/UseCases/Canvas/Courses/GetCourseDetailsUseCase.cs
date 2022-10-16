using MediatR;
using StudentProgress.Core.CanvasApi;
using StudentProgress.Core.CanvasApi.Models;
using System.Threading;

namespace StudentProgress.Core.UseCases.Canvas.Courses;

public class GetCourseDetailsResponse
{
    public string? Name { get; init; }
    public string? CanvasId { get; init; }
    public string? TermName { get; init; }
    public DateTime? TermStartsAt { get; init; }
    public DateTime? TermEndsAt { get; init; }
    public string? SectionCanvasId { get; init; }
    public string? SectionName { get; init; }
    public List<GetCourseDetailsStudent> Students { get; init; } = new();
}

public class GetCourseDetailsStudent
{
    public string? Name { get; init; }
    public string? AvatarUrl { get; init; }
    public string? CanvasId { get; init; }
}

public class GetCourseDetailsUseCase : IUseCaseBase<GetCourseDetailsUseCase.Command, GetCourseDetailsUseCase.Response>
{
    public record Response(IEnumerable<GetCourseDetailsResponse> Courses);
    public record Command(string Id) : IRequest<Response>;
    private readonly ICanvasClient _client;

    private readonly string _query = @"query MyQuery {
course(id: ""{id}"") {
  _id
  name
  term {
    _id
    name
    startAt
    endAt
  }
  sectionsConnection {
    nodes {
      _id
      name
    }
  }
  enrollmentsConnection {
    nodes {
      _id
      user {
        _id
        name
        avatarUrl
      }
      section {
        _id
        name
      }
    }
  }
}
}";

    public GetCourseDetailsUseCase(ICanvasClient client) => _client = client;

    public async Task<Response> Handle(Command command, CancellationToken token)
    {
        var query = _query.Replace("{id}", command.Id);
        var data = await _client.GetAsync<ApiResponse>(query, token);
        var course = data?.Data?.Course;

        return new Response(course?.SectionsConnection?.Nodes.Select(section =>
        {
            return new GetCourseDetailsResponse
            {
                Name = course.Name,
                CanvasId = course.Id,
                SectionName = section.Name,
                TermName = course.Term?.Name,
                TermStartsAt = course.Term?.StartAt,
                TermEndsAt = course.Term?.EndAt,
                SectionCanvasId = section.Id,
                Students = course.EnrollmentsConnection?.Nodes.Where(e => e.Section?.Id == section.Id)
                    .Select(e => new GetCourseDetailsStudent
                    {
                        Name = e.User?.Name, AvatarUrl = e.User?.AvatarUrl, CanvasId = e.User?.Id
                    })
                    .ToList() ?? new List<GetCourseDetailsStudent>()
            };
        }) ?? new List<GetCourseDetailsResponse>());
    }
}

public class ApiResponse
{
    public Course? Course { get; set; }
}