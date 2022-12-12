using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;
using System.Threading;

namespace StudentProgress.Core.UseCases
{
    public class MilestonesCopyFromGroup : IUseCaseBase<MilestonesCopyFromGroup.Command, Result>
    {
        private readonly ProgressContext _db;

        public MilestonesCopyFromGroup(ProgressContext db)
        {
            _db = db;
        }

        public async Task<Result> Handle(Command command, CancellationToken token)
        {
            var groupFrom = Maybe<StudentGroup?>.From(
                    await _db.Groups.Include(g => g.Milestones)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(g => g.Id == command.FromGroupId, token))
                .ToResult("Group to copy from does not exist");
            var groupTo = Maybe<StudentGroup?>.From(
                    await _db.Groups.Include(g => g.Milestones).FirstOrDefaultAsync(g => g.Id == command.ToGroupId))
                .ToResult("Group to copy to does not exist");
            var groupsExist = Result.Combine(groupFrom, groupTo);

            if (groupsExist.IsFailure)
            {
                return groupsExist;
            }

            groupFrom.Value!.Milestones
                .Where(m => groupTo.Value!.Milestones.All(existingMilestone =>
                    existingMilestone.Artefact != m.Artefact &&
                    existingMilestone.LearningOutcome != m.LearningOutcome ||
                    existingMilestone.LearningOutcome == m.LearningOutcome && 
                    existingMilestone.Artefact != m.Artefact))
                .Select(m => new Milestone(m.LearningOutcome, m.Artefact, groupTo.Value!))
                .ToList()
                .ForEach(m => groupTo.Value!.AddMilestone(m));

            await _db.SaveChangesAsync();
            return Result.Success();
        }

        public record Command : IUseCaseRequest<Result>
        {
            public int FromGroupId { get; set; }
            public int ToGroupId { get; set; }
        }
    }
}