using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;

namespace StudentProgress.Core.UseCases
{
    public class MilestoneCreate
    {
        private readonly ProgressContext _context;

        public MilestoneCreate(ProgressContext context)
        {
            _context = context;
        }

        public record Command
        {
            public int? Id { get; init; }
            [Required] public int GroupId { get; init; }
            [Required] public string LearningOutcome { get; init; } = null!;
            [Required] public string Artefact { get; init; } = null!;
        }

        public async Task<Result> HandleAsync(Command command)
        {
            var groupResult = Maybe<StudentGroup>.From(
                await _context.Groups.FirstOrDefaultAsync(g => g.Id == command.GroupId)
            ).ToResult($"Group with ID {command.GroupId} does not exist");
            var learningOutcomeResult = Name.Create(command.LearningOutcome);
            var artefactResult = Name.Create(command.Artefact);
            var validationResult = Result.Combine(groupResult, artefactResult, learningOutcomeResult);

            if (validationResult.IsFailure)
            {
                return validationResult;
            }

            return await groupResult
                .Check(group => group.AddMilestone(new Milestone(learningOutcomeResult.Value, artefactResult.Value)))
                .Tap(async _ => await _context.SaveChangesAsync());
        }
    }
}