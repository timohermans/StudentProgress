using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;
using System.Threading;

namespace StudentProgress.Core.UseCases
{
    public class ProgressGetForCreateOrUpdate : IUseCaseBase<ProgressGetForCreateOrUpdate.Query,
        Result<ProgressGetForCreateOrUpdate.Response>>
    {
        public record Query : IUseCaseRequest<Result<Response>>
        {
            public int? Id { get; set; }
            public int GroupId { get; set; }
            public int StudentId { get; set; }
        }

        public record Response(
            Student Student,
            StudentGroup Group,
            List<Milestone> Milestones,
            List<MilestoneProgress> MilestoneProgresses,
            ProgressCreateOrUpdate.Command Command);

        private readonly ProgressContext _context;

        public ProgressGetForCreateOrUpdate(ProgressContext context)
        {
            _context = context;
        }

        public async Task<Result<Response>> Handle(Query query, CancellationToken token)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == query.StudentId, token);
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == query.GroupId, token);
            var milestones = _context.Milestones
                .Where(m => m.StudentGroup.Id == query.GroupId)
                .OrderBy(m => m.LearningOutcome)
                .ThenBy(m => m.Artefact)
                .ToList();
            var progressUpdates = await _context.ProgressUpdates
                .Include(p => p.MilestonesProgress)
                .ThenInclude(m => m.Milestone)
                .Where(pu => pu.StudentId == query.StudentId && pu.GroupId == query.GroupId)
                .ToListAsync(token);
            var progressUpdate = progressUpdates
                .FirstOrDefault(p => p.Id == (query.Id ?? 0));
            var milestoneProgresses =
                progressUpdates
                    .SelectMany(pu => pu.MilestonesProgress)
                    .ToList();

            if (student == null || group == null || (query.Id != null && progressUpdate == null))
            {
                return Result.Failure<Response>(
                    "Either group and/or student doesn't exist, or you're trying to access a non-existing progress update");
            }

            var command = new ProgressCreateOrUpdate.Command
            {
                Date = progressUpdate?.Date ?? DateTime.Now,
                Feedback = progressUpdate?.Feedback,
                GroupId = group.Id,
                StudentId = student.Id,
                Id = progressUpdate?.Id,
                Feeling = progressUpdate?.ProgressFeeling ?? Feeling.Neutral,
                IsReviewed = progressUpdate?.IsReviewed ?? false,
                Milestones = milestones.Select(milestone =>
                    {
                        var milestoneProgress =
                            progressUpdate?.MilestonesProgress.FirstOrDefault(pu => milestone.Id == pu.Milestone.Id);
                        return new ProgressCreateOrUpdate.MilestoneProgressCommand
                        {
                            Rating = milestoneProgress?.Rating,
                            Comment = milestoneProgress?.Comment,
                            MilestoneId = milestone.Id,
                            Id = milestoneProgress?.Id
                        };
                    })
                    .ToList()
            };

            return Result.Success(new Response(student!, group!, milestones, milestoneProgresses, command));
        }
    }
}