using CSharpFunctionalExtensions;
using StudentProgress.Core.Entities;
using System.ComponentModel.DataAnnotations;
using System.Threading;

namespace StudentProgress.Core.UseCases
{
  public class MilestoneUpdate : IUseCaseBase<MilestoneUpdate.Command, Result>
  {
    private readonly ProgressContext _context;

    public record Command : IUseCaseRequest<Result>
    {
      public int Id { get; init; }
      [Required] public string LearningOutcome { get; init; } = null!;
      [Required] public string Artefact { get; init; } = null!;
    }

    public MilestoneUpdate(ProgressContext context)
    {
      _context = context;
    }

    public async Task<Result> Handle(Command command, CancellationToken token)
    {
      var milestoneResult = Maybe<Milestone>.From(await _context.Milestones.FindAsync(command.Id, token)).ToResult("Milestone doesn't exist");
      var learningOutcomeResult = Name.Create(command.LearningOutcome);
      var artefactResult = Name.Create(command.Artefact);
      var validationResult = Result.Combine(milestoneResult, artefactResult, learningOutcomeResult);

      if (validationResult.IsFailure)
      {
        return validationResult;
      }

      var doesArtefactAlreadyExist = _context.Milestones.Any(m => m.Id != command.Id && m.Artefact == artefactResult.Value && m.LearningOutcome == learningOutcomeResult.Value);
      if (doesArtefactAlreadyExist)
      {
        return Result.Failure("Artefact already exists for that learning outcome");
      }

      milestoneResult.Value.UpdateDetails(learningOutcomeResult.Value, artefactResult.Value);
      await _context.SaveChangesAsync();
      return Result.Success();
    }

  }
}
