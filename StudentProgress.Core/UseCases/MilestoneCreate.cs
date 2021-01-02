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

        public record Request
        {
            [Required] public int GroupId { get; init; }
            [Required] public string Name { get; init; } = null!;
        }

        public async Task<Result> HandleAsync(Request request)
        {
            var groupResult = Maybe<StudentGroup>.From(
                await _context.Groups.FirstOrDefaultAsync(g => g.Id == request.GroupId)
            ).ToResult($"Group with ID {request.GroupId} does not exist");
            var nameResult = Name.Create(request.Name);
            var validationResult = Result.Combine(groupResult, nameResult);

            if (validationResult.IsFailure)
            {
                return validationResult;
            }

            return await groupResult
                .Check(group => group.AddMilestone(new Milestone(nameResult.Value)))
                .Tap(async _ => await _context.SaveChangesAsync());
        }
    }
}