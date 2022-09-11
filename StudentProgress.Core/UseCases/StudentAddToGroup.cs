using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;
using System.ComponentModel.DataAnnotations;
using CSharpFunctionalExtensions;

namespace StudentProgress.Core.UseCases
{
    public class StudentAddToGroup
    {
        private readonly ProgressContext context;

        public StudentAddToGroup(ProgressContext context)
        {
            this.context = context;
        }

        public record Request
        {
            [Required] public string Name { get; init; } = null!;
            [Required] public int GroupId { get; init; }
            public string? AvatarPath { get; init; } = null;
        };

        public async Task<Result> HandleAsync(Request request)
        {
            var studentGroup = Maybe<StudentGroup?>.From(
                    await context.Groups.Include(_ => _.Students)
                        .FirstOrDefaultAsync(group => group.Id == request.GroupId))
                .ToResult("Group not found");
            var student = await GetOrCreateUserFrom(request.Name);
            var result = Result.Combine(studentGroup, student);

            if (result.IsFailure)
                return Result.Failure(result.Error);

            student.Value.UpdateAvatar(request.AvatarPath);

            return await studentGroup.Value?.AddStudent(student.Value)
                .Tap(async () => await context.SaveChangesAsync())!;
        }
        
        private async Task<Result<Student>> GetOrCreateUserFrom(string name)
        {
            var student = await context.Students.FirstOrDefaultAsync(s => s.Name == name);

            if (student == null)
            {
                student = new Student(name);
                await context.Students.AddAsync(student);
            }

            return Result.Success(student);
        }
    }
}