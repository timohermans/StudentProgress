using StudentProgress.Core.CanvasApi;
using StudentProgress.Core.CanvasApi.Models;

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

public class GetCourseDetailsUseCase
{
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

    public async Task<IEnumerable<GetCourseDetailsResponse>> Execute(string id)
    {
        var query = _query.Replace("{id}", id);
        var data = await _client.GetAsync<Response>(query);
        var course = data?.Data?.Course;

        return course?.SectionsConnection?.Nodes.Select(section =>
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
        }) ?? new List<GetCourseDetailsResponse>();
    }
}

public class Response
{
    public Course? Course { get; set; }
}