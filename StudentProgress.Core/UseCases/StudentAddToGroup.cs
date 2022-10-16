using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;
using System.ComponentModel.DataAnnotations;
using CSharpFunctionalExtensions;
using System.Threading;

namespace StudentProgress.Core.UseCases
{
    public class StudentAddToGroup : IUseCaseBase<StudentAddToGroup.Request, Result<int>>
    {
        private readonly ProgressContext context;

        public StudentAddToGroup(ProgressContext context)
        {
            this.context = context;
        }

        public record Request : IUseCaseRequest<Result<int>>
        {
            [Required] public string Name { get; init; } = null!;
            [Required] public int GroupId { get; init; }
            public string? AvatarPath { get; init; } = null;
        };

        public async Task<Result<int>> Handle(Request request, CancellationToken token)
        {
            var studentGroup = Maybe<StudentGroup?>.From(
                    await context.Groups.Include(_ => _.Students)
                        .FirstOrDefaultAsync(group => group.Id == request.GroupId, token))
                .ToResult("Group not found");
            var student = await GetOrCreateUserFrom(request.Name, token);
            var result = Result.Combine(studentGroup, student);

            if (result.IsFailure)
                return Result.Failure<int>(result.Error);

            return await studentGroup.Value?.AddStudent(student.Value)
                .Tap(async () => await context.SaveChangesAsync(token))
                .Map(() => Task.FromResult(student.Value.Id))!;
        }

        private async Task<Result<Student>> GetOrCreateUserFrom(string name, CancellationToken token)
        {
            var student = await context.Students.FirstOrDefaultAsync(s => s.Name == name, token);

            if (student == null)
            {
                student = new Student(name);
                await context.Students.AddAsync(student, token);
            }

            return Result.Success(student);
        }
    }
}