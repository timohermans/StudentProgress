using System.IO;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using CSharpFunctionalExtensions;
using StudentProgress.Core.Entities;
using System.Threading;

namespace StudentProgress.Core.UseCases;

public class GroupImportFromCanvas : IUseCaseBase<GroupImportFromCanvas.Request, Result>
{
    public class Request : IUseCaseRequest<Result>
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

    public class GetCourseDetailsStudent
    {
        public required string Name { get; init; }
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

    public async Task<Result> Handle(Request request, CancellationToken token)
    {
        var groupName = $"{request.Name} - {request.SectionName} - {request.TermName}";
        var response = await new GroupCreate(_db).Handle(new GroupCreate.Request
        {
            Name = groupName,
            StartDate = request.TermStartsAt,
            StartPeriod = request.TermStartsAt
        }, token);
        var groupResult = response;

        if (groupResult.IsFailure && !groupResult.Error.Contains("already exists")) return groupResult;
        var group = await _db
            .Groups
            .Include(g => g.Students)
            .FirstAsync(g => g.Name == groupName, token);
        var studentNamesRequest = request.Students.Select(s => s.Name).ToList();
        var studentsAlreadyInDb = await _db.Students.Where(s => studentNamesRequest.Contains(s.Name)).ToListAsync(token);

        var relativeAvatarLocation = Path.Combine("images", "avatars");
        var imageLocation = Path.Combine(_config.MediaLocation, relativeAvatarLocation);
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
            var fileResponse = await _client.GetAsync(studentRequest.AvatarUrl, token);
            if (fileResponse.IsSuccessStatusCode)
            {
                var filePath = Path.Combine(_config.MediaLocation, relativeAvatarLocation, fileName);
                await using var fs = new FileStream(filePath, FileMode.Create);
                await fileResponse.Content.CopyToAsync(fs, token);
                student.UpdateAvatar(Path.Combine(relativeAvatarLocation, fileName));
            }
        }

        await _db.SaveChangesAsync(token);

        return Result.Success();
    }
}