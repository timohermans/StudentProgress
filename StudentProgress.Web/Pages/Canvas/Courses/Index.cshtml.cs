using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Course = StudentProgress.Core.CanvasApi.Models.Course;
using ICanvasClient = StudentProgress.Core.CanvasApi.ICanvasClient;

namespace StudentProgress.Web.Pages.Canvas.Courses;

public class Index : PageModel
{
    public record ApiResponse(IEnumerable<Course>? AllCourses);

    private readonly ICanvasClient _client;

    public IEnumerable<Course> Courses { get; private set; } = null!;


    public Index(ICanvasClient client)
    {
        _client = client;
    }

    public async Task OnGetAsync(CancellationToken token)
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
        Courses = data?.Data?.AllCourses?.OrderByDescending(c => c.Term?.StartAt).ToList() ?? [];
    }
}