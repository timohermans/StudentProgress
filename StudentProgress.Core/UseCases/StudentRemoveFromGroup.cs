using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;
using System.Threading;

namespace StudentProgress.Core.UseCases
{
    public class StudentRemoveFromGroup : IUseCaseBase<StudentRemoveFromGroup.Command, Result>
    {
        private readonly ProgressContext _ucContext;

        public StudentRemoveFromGroup(ProgressContext ucContext)
        {
            _ucContext = ucContext;
        }

        public async Task<Result> Handle(Command command, CancellationToken token)
        {
            var studentResult = Maybe<Student>.From(await _ucContext.Students.FindAsync(command.StudentId, token))
                .ToResult("Student doesn't exist");
            var groupResult = Maybe<StudentGroup>.From(
                    await _ucContext
                        .Groups
                        .Include(g => g.Students)
                        .FirstOrDefaultAsync(g => g.Id == command.GroupId, token))
                .ToResult("Student doesn't exist");
            var validationResult = Result.Combine(studentResult, groupResult);

            if (validationResult.IsFailure) return validationResult;

            var progressUpdatesToRemove =
                _ucContext.ProgressUpdates.Include(p => p.MilestonesProgress).Where(p =>
                    p.StudentId == command.StudentId && p.GroupId == command.GroupId);
            _ucContext.RemoveRange(progressUpdatesToRemove);

            groupResult.Value.RemoveStudent(studentResult.Value);

            await _ucContext.SaveChangesAsync();

            return Result.Success();
        }

        public record Command : IUseCaseRequest<Result>
        {
            public int GroupId { get; set; }
            public int StudentId { get; set; }
        }
    }
}