using System.IO;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using CSharpFunctionalExtensions;
using StudentProgress.Core.Entities;

namespace StudentProgress.Core.UseCases;

public class GroupImportFromCanvas : UseCaseBase<GroupImportFromCanvas.Request, Result>
{
    public class Request
    {
        public string Name { get; init; }
        public string? CanvasId { get; init; }
        public string? TermName { get; init; }
        public DateTime TermStartsAt { get; init; }
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

    public class GetCourseDetailsStudent
    {
        public string Name { get; init; }
        public string? AvatarUrl { get; init; }
        public string? CanvasId { get; init; }
    }

    private readonly ProgressContext _db;
    private readonly ICoreConfiguration _config;
    private readonly HttpClient _client;

    public GroupImportFromCanvas(ProgressContext db, ICoreConfiguration config, HttpClient client)
    {
        _db = db;
        _config = config;
        _client = client;
    }

    public async Task<Result> HandleAsync(Request request)
    {
        var groupResult = await new GroupCreate(_db).HandleAsync(new GroupCreate.Request
        {
            Name = request.Name,
            StartDate = request.TermStartsAt,
            StartPeriod = request.TermStartsAt
        });

        if (groupResult.IsFailure && !groupResult.Error.Contains("already exists")) return groupResult;
        var group = await _db
            .Groups
            .Include(g => g.Students)
            .FirstAsync(g => g.Name == request.Name);
        var studentNamesRequest = request.Students.Select(s => s.Name).ToList();
        var studentsAlreadyInDb = await _db.Students.Where(s => studentNamesRequest.Contains(s.Name)).ToListAsync();

        var imageLocation = Path.Combine(_config.MediaLocation, "images", "avatars");
        if (!Directory.Exists(imageLocation)) Directory.CreateDirectory(imageLocation);
        
        foreach (var studentRequest in request.Students)
        {
            Student student;
            var studentInGroup = group.Students.FirstOrDefault(s => s.ExternalId == studentRequest.CanvasId);
            var studentInDb = studentsAlreadyInDb.FirstOrDefault(s => s.ExternalId == studentRequest.CanvasId);

            if (studentInGroup is not null) student = studentInGroup;
            if (studentInDb is not null)
            {
                group.AddStudent(studentInDb);
                student = studentInDb;
            }
            else
            {
                student = new Student(studentRequest.Name, studentRequest.CanvasId);
                _db.Students.Add(student);
                group.AddStudent(student);
            }

            if (studentRequest.AvatarUrl is null) continue;
            var fileName = $"{student.ExternalId}-canvas.png";
            var fileResponse = await _client.GetAsync(studentRequest.AvatarUrl);
            if (fileResponse.IsSuccessStatusCode)
            {
                var filePath = Path.Combine(_config.MediaLocation, "images", "avatars", fileName);
                await using var fs = new FileStream(filePath, FileMode.Create);
                await fileResponse.Content.CopyToAsync(fs);
                student.UpdateAvatar(fileName);
            }
        }

        await _db.SaveChangesAsync();

        return Result.Success();
    }
}