using CSharpFunctionalExtensions;
using StudentProgress.Core.Entities;

namespace StudentProgress.Core.UseCases
{
    public class MilestoneDelete : UseCaseBase<MilestoneDelete.Command, Result>
    {
        private readonly ProgressContext _db;

        public MilestoneDelete(ProgressContext db)
        {
            _db = db;
        }

        public async Task<Result> HandleAsync(Command command)
        {
            var milestone = await _db.Milestones.FindAsync(command.Id);

            _db.Milestones.Remove(milestone);

            await _db.SaveChangesAsync();
            return Result.Success();
        }

        public record Command
        {
            public int Id { get; set; }
        }
    }
}