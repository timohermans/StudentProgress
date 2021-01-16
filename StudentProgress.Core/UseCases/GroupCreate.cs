using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;

namespace StudentProgress.Core.UseCases
{
    public class GroupCreate
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

        public async Task<Result> HandleAsync(Request request)
        {
            var name = Name.Create(request.Name);
            var periodResult = Period.Create(request.StartDate);
            var validationResult = Result.Combine(name, periodResult);

            if (validationResult.IsFailure)
            {
                return validationResult;
            }

            return await name
                .Check(IsGroupNew)
                .Tap(async () => await context.Groups.AddAsync(new StudentGroup(name.Value, periodResult.Value, request.Mnemonic)))
                .Tap(() => context.SaveChangesAsync());
        }

        private async Task<Result> IsGroupNew(Name name)
        {
            return Result.SuccessIf(
                (await context.Groups.FirstOrDefaultAsync(g => g.Name == name)) == null,
                "Group already exists");
        }
    }
}