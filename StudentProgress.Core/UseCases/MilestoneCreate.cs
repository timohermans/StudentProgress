using System.ComponentModel.DataAnnotations;
using System.Threading;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;

namespace StudentProgress.Core.UseCases
{
    public class MilestoneCreate : IUseCaseBase<MilestoneCreate.Command, Result<int>>
    {
        private readonly ProgressContext _context;

        public MilestoneCreate(ProgressContext context)
        {
            _context = context;
        }

        public record Command : IUseCaseRequest<Result<int>>
        {
            public int? Id { get; init; }
            [Required] public int GroupId { get; init; }
            [Required] public string LearningOutcome { get; init; } = null!;
            [Required] public string Artefact { get; init; } = null!;
        }

        public async Task<Result<int>> Handle(Command command, CancellationToken token)
        {
            var groupResult = Maybe<StudentGroup>.From(
                await _context.Groups.FirstOrDefaultAsync(g => g.Id == command.GroupId, token)
            ).ToResult($"Group with ID {command.GroupId} does not exist");
            var learningOutcomeResult = Name.Create(command.LearningOutcome);
            var artefactResult = Name.Create(command.Artefact);
            var validationResult = Result.Combine(groupResult, artefactResult, learningOutcomeResult);

            if (validationResult.IsFailure)
            {
                return validationResult.ConvertFailure<int>();
            }

            var doesArtefactAlreadyExist = _context.Milestones.Any(m =>
                m.StudentGroupId == command.GroupId && m.Artefact == artefactResult.Value &&
                m.LearningOutcome == learningOutcomeResult.Value);

            if (doesArtefactAlreadyExist)
            {
                return Result.Failure<int>("Artefact already exists for that learning outcome");
            }

            var milestone = new Milestone(learningOutcomeResult.Value, artefactResult.Value, groupResult.Value);
            return await groupResult
                .Check(g => g.AddMilestone(milestone))
                .Tap(async _ => await _context.SaveChangesAsync(token))
                .Map(_ => milestone.Id);
        }
    }
}