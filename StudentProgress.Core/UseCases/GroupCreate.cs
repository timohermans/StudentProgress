using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;

namespace StudentProgress.Core.UseCases
{
    public class GroupCreate : IUseCaseBase<GroupCreate.Request, Result<int>>
    {
        private readonly ProgressContext context;

        public GroupCreate(ProgressContext context)
        {
            this.context = context;
        }

        public record Request : IUseCaseRequest<Result<int>>
        {
            [Required] public string Name { get; init; } = null!;

            public string? Mnemonic { get; init; }
            public DateTime StartPeriod { get; init; }
            [DisplayName("Starting date")]
            public DateTime StartDate { get; init; }
        }

        public async Task<Result<int>> Handle(Request request, CancellationToken token)
        {
            var name = Name.Create(request.Name);
            var periodResult = Period.Create(request.StartDate);
            var validationResult = Result.Combine(name, periodResult);

            if (validationResult.IsFailure)
            {
                return validationResult.ConvertFailure<int>();
            }

            var group = Result.Success(new StudentGroup(name.Value, periodResult.Value, request.Mnemonic));
            var groupIdResult = await group
                .Ensure(g => IsGroupNew(g.Name, g.Period))
                .Tap(async (g) => await context.Groups.AddAsync(g, token))
                .Tap(() => context.SaveChangesAsync(token))
                .Map(g => g.Id);
            return groupIdResult;
        }

        private async Task<Result> IsGroupNew(Name name, Period period)
        {
            return Result.SuccessIf(
                await context.Groups.FirstOrDefaultAsync(g => g.Name == name && g.Period == period) == null,
                "Group already exists");
        }
    }
}