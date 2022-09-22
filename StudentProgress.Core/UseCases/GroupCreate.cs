using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;

namespace StudentProgress.Core.UseCases
{
    public class GroupCreate : UseCaseBase<GroupCreate.Request, Result<int>>
    {
        private readonly ProgressContext context;

        public GroupCreate(ProgressContext context)
        {
            this.context = context;
        }

        public record Request
        {
            [Required] public string Name { get; init; } = null!;

            public string? Mnemonic { get; init; }
            public DateTime StartPeriod { get; init; }
            [DisplayName("Starting date")]
            public DateTime StartDate { get; init; }
        }

        public async Task<Result<int>> HandleAsync(Request request)
        {
            var name = Name.Create(request.Name);
            var periodResult = Period.Create(request.StartDate);
            var validationResult = Result.Combine(name, periodResult);

            if (validationResult.IsFailure)
            {
                return validationResult.ConvertFailure<int>();
            }

            var group = Result.Success(new StudentGroup(name.Value, periodResult.Value, request.Mnemonic));
            return await group
                .Ensure(g => IsGroupNew(g.Name, g.Period))
                .Tap(async (g) => await context.Groups.AddAsync(g))
                .Tap(() => context.SaveChangesAsync())
                .Map(g => g.Id);
        }

        private async Task<Result> IsGroupNew(Name name, Period period)
        {
            return Result.SuccessIf(
                await context.Groups.FirstOrDefaultAsync(g => g.Name == name && g.Period == period) == null,
                "Group already exists");
        }
    }
}