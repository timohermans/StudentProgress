using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;

namespace StudentProgress.Core.UseCases
{
    public class ProgressCreate
    {
        private readonly ProgressContext context;


        public ProgressCreate(ProgressContext context)
        {
            this.context = context;
        }

        public record MilestoneProgress
        {
            public int Id { get; set; }
            public Rating? Rating { get; set; }
        }

        public record Request
        {
            [Required] public int StudentId { get; set; }
            [Required] public int GroupId { get; set; }
            [Required] public Feeling Feeling { get; set; }
            [Required] [DataType(DataType.Date)] public DateTime Date { get; set; }
            public string? Feedback { get; set; }
            public string? Feedup { get; set; }
            public string? Feedforward { get; set; }
            public List<MilestoneProgress> Milestones { get; set; } = new List<MilestoneProgress>();
        }

        public async Task<Result> HandleAsync(Request progress)
        {
            var student = Maybe<Student>.From(
                await context.Students.FirstOrDefaultAsync(s => s.Id == progress.StudentId)
            ).ToResult("Student does not exist");
            var group = Maybe<StudentGroup>.From(
                await context.Groups.FirstOrDefaultAsync(g => g.Id == progress.GroupId)
            ).ToResult("Group does not exist");
            var result = Result.Combine(student, group);

            if (result.IsFailure) return result;

            await context.ProgressUpdates.AddAsync(new ProgressUpdate(
                student.Value,
                group.Value,
                progress.Feedback,
                progress.Feedup,
                progress.Feedforward,
                progress.Feeling,
                progress.Date));
            await context.SaveChangesAsync();
            return Result.Success();
        }
    }
}