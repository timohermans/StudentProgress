using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace StudentProgress.Core.UseCases
{
    public class ProgressGetForStudentInGroup
    {
        public record Request(int? GroupId, int? StudentId);
        public record Response
        {
            public int StudentId { get; init; }
            public int GroupId { get; init; }
            public string? Name { get; init; }
            public string? GroupName { get; init; }
            public IEnumerable<ProgressUpdateResponse> ProgressUpdates { get; init; }

            public Response()
            {
                ProgressUpdates = new List<ProgressUpdateResponse>();
            }

            public record MilestoneProgressResponse
            {
                public string? Comment { get; init; }
                public Rating Rating { get; init; }
                public string Artefact { get; init; }
                public string LearningOutcome { get; init; }

                public MilestoneProgressResponse(string? comment, Rating rating, string artefact, string learningOutcome)
                {
                    Comment = comment;
                    Rating = rating;
                    Artefact = artefact;
                    LearningOutcome = learningOutcome;
                }
            }

            public record ProgressUpdateResponse
            {
                public int Id { get; init; }
                public string? Feedback { get; init; }
                public DateTime UpdatedAt { get; init; }
                public DateTime CreatedAt { get; init; }
                [DataType(DataType.Date)]
                public DateTime Date { get; init; }
                public Feeling Feeling { get; init; }
                public IEnumerable<MilestoneProgressResponse> MilestoneProgresses { get; init; } = Enumerable.Empty<MilestoneProgressResponse>();
            }
        }

        private readonly ProgressContext _context;
        public ProgressGetForStudentInGroup(ProgressContext context)
        {
            _context = context;
        }

        public async Task<Response> HandleAsync(Request request)
        {
            var student = await _context.Students
                .Include(_ => _.ProgressUpdates)
                .ThenInclude(_ => _.MilestonesProgress)
                .ThenInclude(_ => _.Milestone)
                .FirstOrDefaultAsync(g => g.Id == (request.StudentId ?? 0));
            var group = _context.Groups.FirstOrDefault(g => g.Id == (request.GroupId ?? 0));

            if (student == null || group == null) throw new InvalidOperationException("Must supply a valid student and group");

            var progressForGroup = student.ProgressUpdates
                .Where(p => p.GroupId == request.GroupId)
                .OrderByDescending(p => p.Date);

            return new Response
            {
                GroupId = group.Id,
                StudentId = student.Id,
                Name = student.Name,
                GroupName = group.Name,
                ProgressUpdates = progressForGroup.Select(p => new Response.ProgressUpdateResponse
                {
                    Id = p.Id,
                    Feedback = p.Feedback,
                    Feeling = p.ProgressFeeling,
                    Date = p.Date,
                    UpdatedAt = p.UpdatedDate,
                    CreatedAt = p.CreatedDate,
                    MilestoneProgresses = p.MilestonesProgress
                        .Select(m => new Response.MilestoneProgressResponse(m.Comment, m.Rating, m.Milestone.Artefact, m.Milestone.LearningOutcome))
                        .ToList()
                })
            };
        }
    }
}
