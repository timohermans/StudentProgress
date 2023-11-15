using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Web.Lib.CanvasApi.Models;
using StudentProgress.Web.Lib.Data;
using StudentProgress.Web.Lib.Infrastructure;
using StudentProgress.Web.Models;
using StudentProgress.Web.Models.Values;
using ICanvasClient = StudentProgress.Web.Lib.CanvasApi.ICanvasClient;

namespace StudentProgress.Web.Pages.Canvas.Courses;

public class Details : PageModel
{
    public new class Request
    {
        public required string Name { get; init; }
        public string? CanvasId { get; init; }
        public string? TermName { get; init; }
        public required DateTime TermStartsAt { get; init; }
        public DateTime? TermEndsAt { get; init; }
        public string? SectionCanvasId { get; init; }
        public string? SectionName { get; init; }
        public List<GetCourseDetailsStudent> Students { get; init; } = new();
    }

    public class ImportSemesterStudent
    {
        public string? Name { get; init; }
        public string? AvatarUrl { get; init; }
        public string? CanvasId { get; init; }
    }

    public record ApiResponse(Course? Course);

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

    private readonly ICanvasClient _canvasClient;
    private readonly WebContext _db;
    private readonly ICoreConfiguration _config;
    private readonly HttpClient _httpClient;

    public List<GetCourseDetailsResponse> Semesters { get; set; } = new();
    public List<ErrorResult> Errors { get; set; } = new();

    [BindProperty] public Request Semester { get; set; } = default!;

    public Details(ICanvasClient canvasClient, WebContext db, ICoreConfiguration config, HttpClient httpClient)
    {
        _canvasClient = canvasClient;
        _config = config;
        _httpClient = httpClient;
        _db = db;
    }

    public async Task OnGetAsync(string id, CancellationToken token)
    {
        var query = _query.Replace("{id}", id);
        var data = await _canvasClient.GetAsync<Details.ApiResponse>(query, token);
        var course = data?.Data?.Course;

        Semesters = course?.SectionsConnection?.Nodes.Select(section =>
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
                    .OrderBy(p => p.Name)
                    .ToList() ?? new List<GetCourseDetailsStudent>()
            };
        }).ToList() ?? new List<GetCourseDetailsResponse>();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken token)
    {
        var adventureName = Semester.Name;

        if (Semester.Name != Semester.SectionName)
        {
            adventureName = $"{Semester.Name} - {Semester.SectionName}";
        }

        /*
         * select a.id, p.id, p.name, p.externalId
         * from adventure a
         * left join person p on a.id = p.adventureId
         * where a.name = "{name}" and a.dateStart = "{date}"
         *
         * insert into adventure (name, dateStart) values ("{name}", "{date}") returning id;
         */
        Models.Adventure? adventure = await _db.Adventures
            .Include(a => a.People)
            .FirstOrDefaultAsync(a => a.Name == adventureName && a.DateStart == Semester.TermStartsAt,
                cancellationToken: token);

        if (adventure == null)
        {
            adventure = new Models.Adventure
            {
                Name = adventureName,
                DateStart = Semester.TermStartsAt
            };
            await _db.Adventures.AddAsync(adventure, token);
        }

        var peopleNamesToAdd = Semester.Students.Select(s => s.Name).ToList();
        var peopleInDb =
            await _db.People.Where(p => peopleNamesToAdd.Contains(p.LastName + ", " + p.FirstName + " " + p.Initials))
                .ToListAsync(token);

        var relativeAvatarLocation = Path.Combine("images", "avatars");
        var imageLocation = Path.Combine(_config.MediaLocation, relativeAvatarLocation);
        if (!Directory.Exists(imageLocation)) Directory.CreateDirectory(imageLocation);

        foreach (var studentRequest in Semester.Students.DistinctBy(s => s.Name))
        {
            Person person;
            var personInAdventure = adventure.People.FirstOrDefault(s => s.ExternalId == studentRequest.CanvasId);
            var personInDb = peopleInDb.FirstOrDefault(s => s.Name == studentRequest.Name);

            if (personInAdventure is not null)
            {
                person = personInAdventure;
            }
            else if (personInDb is not null)
            {
                adventure.People.Add(personInDb);
                person = personInDb;
            }
            else
            {
                var name = NameFromCanvas.Create(studentRequest.Name);
                // TODO: error handling
                person = new Person
                {
                    FirstName = name.Data.FirstName,
                    LastName = name.Data.LastName,
                    Initials = name.Data.Initials,
                    ExternalId = studentRequest.CanvasId
                };
                _db.People.Add(person);
                adventure.People.Add(person);
            }

            if (studentRequest.AvatarUrl is null) continue;
            var fileName = $"{person.ExternalId}-canvas.png";
            var fileResponse = await _httpClient.GetAsync(studentRequest.AvatarUrl, token);
            if (fileResponse.IsSuccessStatusCode)
            {
                var filePath = Path.Combine(_config.MediaLocation, relativeAvatarLocation, fileName);
                await using var fs = new FileStream(filePath, FileMode.Create);
                await fileResponse.Content.CopyToAsync(fs, token);
                person.AvatarPath = Path.Combine(relativeAvatarLocation, fileName);
            }
        }

        await _db.SaveChangesAsync(token);

        return RedirectToPage("/Index");
    }
}