using CSharpFunctionalExtensions;
using StudentProgress.Core.Entities;
using System.Threading;

namespace StudentProgress.Core.UseCases
{
    public class MilestoneDelete : IUseCaseBase<MilestoneDelete.Command, Result>
    {
        private readonly ProgressContext _db;

        public MilestoneDelete(ProgressContext db)
        {
            _db = db;
        }

        public async Task<Result> Handle(Command command, CancellationToken token)
        {
            var milestone = await _db.Milestones.FindAsync(command.Id);

            if (milestone == null)
            {
                return Result.Failure("Milestone doesn't exist");
            }

            _db.Milestones.Remove(milestone);

            await _db.SaveChangesAsync(token);
            return Result.Success();
        }

        public record Command : IUseCaseRequest<Result>
        {
            public int Id { get; set; }
        }
    }
}