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
        private readonly ProgressContext _context;


        public ProgressCreate(ProgressContext context)
        {
            this._context = context;
        }

        public record MilestoneProgressCommand
        {
            public int Id { get; set; }
            public Rating? Rating { get; set; }
            public string? Comment { get; set; }
        }

        public record Command
        {
            [Required] public int StudentId { get; set; }
            [Required] public int GroupId { get; set; }
            [Required] public Feeling Feeling { get; set; }
            [Required] [DataType(DataType.Date)] public DateTime Date { get; set; }
            public string? Feedback { get; set; }
            public string? Feedup { get; set; }
            public string? Feedforward { get; set; }
            public List<MilestoneProgressCommand> Milestones { get; set; } = new List<MilestoneProgressCommand>();
        }

        public async Task<Result> HandleAsync(Command command)
        {
            var student = Maybe<Student>.From(
                await _context.Students.FirstOrDefaultAsync(s => s.Id == command.StudentId)
            ).ToResult("Student does not exist");
            var group = Maybe<StudentGroup>.From(
                await _context.Groups.FirstOrDefaultAsync(g => g.Id == command.GroupId)
            ).ToResult("Group does not exist");
            var milestonesProgress = await GetMilestonesFrom(command.Milestones);
            var result = Result.Combine(student, group, milestonesProgress);

            if (result.IsFailure) return result;

            var progressUpdate = new ProgressUpdate(
                student.Value,
                group.Value,
                command.Feedback,
                command.Feedup,
                command.Feedforward,
                command.Feeling,
                command.Date);
            progressUpdate.AddMilestones(milestonesProgress.Value);

            await _context.ProgressUpdates.AddAsync(progressUpdate);
            await _context.SaveChangesAsync();
            return Result.Success();
        }

        private async Task<Result<List<MilestoneProgress>>> GetMilestonesFrom(List<MilestoneProgressCommand> milestonesProgress)
        {
            var milestonesToAdd = milestonesProgress.Where(m => m.Rating != null).ToList();
            var milestoneIds = milestonesToAdd.Select(m => m.Id);
            var milestones = await _context.Milestones.Where(m => milestoneIds.Contains(m.Id)).ToListAsync();
            var milestoneProgressWithRating = milestones
                .Select(m =>
                {
                    var milestone = milestonesToAdd.FirstOrDefault(mp => mp.Id == m.Id);
                    return new MilestoneProgress(milestone?.Rating ?? Rating.Undefined, m, milestone?.Comment);
                })
                .ToList();

            return Result.Success(milestoneProgressWithRating);
        }
    }
}