using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;

namespace StudentProgress.Core.UseCases
{
    public class MilestonesUpdateLearningOutcome
    {
        private readonly ProgressContext _dbContext;

        public MilestonesUpdateLearningOutcome(ProgressContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<Result> HandleAsync(Command command)
        {
            var group = Maybe<StudentGroup>.From(await _dbContext.Groups.FindAsync(command.GroupId))
                .ToResult("Group doesn't exist");
            var learningOutcomeResult = Name.Create(command.LearningOutcome);
            var milestoneIdsResult =
                Result.FailureIf(command.MilestoneIds.Length == 0, "At least one milestone ID is required");
            var validationResult = Result.Combine(group, learningOutcomeResult, milestoneIdsResult);

            if (validationResult.IsFailure)
            {
                return validationResult;
            }

            var milestones = await _dbContext.Milestones.Where(m => command.MilestoneIds.Contains(m.Id)).ToListAsync();
            var milestonesOfLearningOutcome = await _dbContext.Milestones
                .Where(m => m.StudentGroupId == command.GroupId && m.LearningOutcome == command.LearningOutcome)
                .ToListAsync();

            return await Result.Combine(
                milestones
                    .Where(m =>
                        milestonesOfLearningOutcome.All(ml => ml.Artefact != m.Artefact) &&
                        !milestones.Any(m2 => m2.Id != m.Id && m2.Artefact == m.Artefact))
                    .Select(m => m.UpdateDetails(learningOutcomeResult.Value, m.Artefact))
            ).Tap(async () => await _dbContext.SaveChangesAsync());
        }

        public record Command
        {
            public int GroupId { get; set; }
            public string LearningOutcome { get; set; } = null!;
            public int[] MilestoneIds { get; set; }
        }
    }
}