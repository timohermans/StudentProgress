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

    public GroupImportFromCanvas(ProgressContext db) => _db = db;

    public async Task<Result> HandleAsync(Request request)
    {
        var group = await new GroupCreate(_db).HandleAsync(new GroupCreate.Request
        {
            Name = request.Name,
            StartDate = request.TermStartsAt,
            StartPeriod = request.TermStartsAt
        });

        foreach (var student in request.Students)
        {
            var addStudentUc = new StudentAddToGroup(_db);
            
            // TODO: download and save picture
            var pathToAvatar = "todo.png";
            
            await addStudentUc.HandleAsync(new StudentAddToGroup.Request
            {
                Name = student.Name,
                GroupId = group.Value.Id ,
                AvatarPath = pathToAvatar
            });
        }

        await _db.SaveChangesAsync();

        return Result.Success();
    }
}