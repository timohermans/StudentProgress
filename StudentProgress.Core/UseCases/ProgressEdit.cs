using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using StudentProgress.Core.Entities;

namespace StudentProgress.Core.UseCases
{
    public class ProgressEdit
    {
        private readonly ProgressContext context;

        public ProgressEdit(ProgressContext context)
        {
            this.context = context;
        }

        public record Request
        {
            [Required] public int Id { get; init; }
            [Required] public Feeling Feeling { get; init; }
            [Required] [DataType(DataType.Date)] public DateTime Date { get; init; }
            public string? Feedback { get; init; }
            public string? Feedup { get; init; }
            public string? Feedforward { get; init; }
        }

        public async Task<Result> HandleAsync(Request request)
        {
            var progress = Maybe<ProgressUpdate>.From(
                await context.ProgressUpdates.FindAsync(request.Id)
            );

            return await progress
                .ToResult("Progress doesn't exist")
                .Check(p =>
                    p.Update(request.Feeling, request.Date, request.Feedback, request.Feedup, request.Feedforward))
                .Tap(() => context.SaveChangesAsync());
        }
    }
}