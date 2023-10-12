﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Course = StudentProgress.Web.Lib.CanvasApi.Models.Course;
using ICanvasClient = StudentProgress.Web.Lib.CanvasApi.ICanvasClient;

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
        Courses = data?.Data?.AllCourses?.OrderByDescending(c => c.Term?.StartAt).ToList() ?? new List<Course>();
    }
}