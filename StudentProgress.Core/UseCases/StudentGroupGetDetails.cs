using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;

namespace StudentProgress.Core.UseCases
{
    public class StudentGroupGetDetails
    {
        private readonly ProgressContext _context;

        public StudentGroupGetDetails(ProgressContext context) => _context = context;

        public record Request(int Id);

        public record Response
        {
            public int Id { get; init; }
            public string Name { get; init; } = null!;
            public string? Mnemonic { get; init; }
            [Display(Name = "Created On")] public DateTime CreatedAt { get; init; }
            [Display(Name = "Last name change")] public DateTime UpdatedAt { get; init; }
            public IList<StudentsResponse> Students { get; set; } = new List<StudentsResponse>();
            public IList<MilestoneResponse> Milestones { get; set; } = new List<MilestoneResponse>();
            public Period Period { get; init; } = null!;

            public record MilestoneResponse
            {
                public int Id { get; init; }

                [DisplayName("Learning Outcome Artefact")]
                public string Artefact { get; init; } = null!;

                [DisplayName("Learning Outcome")] public string LearningOutcome { get; init; } = null!;
            }

            public record StudentsResponse
            {
                public int Id { get; init; }
                public string Name { get; init; } = null!;
                [Display(Name = "#")] public int AmountOfProgressItems { get; init; }
                [Display(Name = "Latest Feeling")] public Feeling? FeelingOfLatestProgress { get; init; }

                [Display(Name = "Last Time")]
                [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
                public DateTime? LastUpdateDate { get; init; }

                [Display(Name = "Last Feedback")] public string? LastFeedback { get; init; }

                public IList<ProgressUpdateResponse> ProgressUpdates { get; set; } = new List<ProgressUpdateResponse>();
            }
        }

        public record ProgressUpdateResponse(int Id, DateTime Date, Feeling Feeling, int StudentId, int GroupId);

        public async Task<Response?> HandleAsync(Request request)
        {
            if (!await DoesGroupExist(request.Id))
            {
                return null;
            }

            return await GetGroupWithStudentDataOfLatestFeedback(request.Id);
        }

        private async Task<bool> DoesGroupExist(int groupId)
        {
            return await _context.Groups.AnyAsync(g => g.Id == groupId);
        }

        private async Task<Response> GetGroupWithStudentDataOfLatestFeedback(int groupId)
        {
            var group = await _context.Groups.Include(g => g.Milestones).FirstOrDefaultAsync(g => g.Id == groupId);
            var students = await _context.Students.Where(s => s.StudentGroups.Any(g => g.Id == groupId)).ToListAsync();
            var progressUpdates = await _context.ProgressUpdates.Where(pu =>
                pu.GroupId == groupId && students.Select(s => s.Id).Contains(pu.StudentId)).ToListAsync();

            return new Response
            {
                Id = group.Id,
                Mnemonic = group.Mnemonic,
                Name = group.Name,
                Period = group.Period,
                CreatedAt = group.CreatedDate,
                UpdatedAt = group.UpdatedDate,
                Milestones = group.Milestones.Select(m => new Response.MilestoneResponse
                {
                    Id = m.Id,
                    Artefact = m.Artefact,
                    LearningOutcome = m.LearningOutcome
                }).ToList(),
                Students = students.Select(s =>
                {
                    var progressUpdatesOfStudent = progressUpdates.Where(pu => pu.StudentId == s.Id)
                        .OrderBy(pu => pu.Date)
                        .ToList();
                    var lastProgressUpdate = progressUpdates
                        .LastOrDefault();

                    return new Response.StudentsResponse
                    {
                        Id = s.Id,
                        Name = s.Name,
                        LastUpdateDate = lastProgressUpdate?.Date,
                        LastFeedback = lastProgressUpdate?.Feedback,
                        FeelingOfLatestProgress = lastProgressUpdate?.ProgressFeeling,
                        AmountOfProgressItems = progressUpdatesOfStudent.Count,
                        ProgressUpdates = progressUpdatesOfStudent.Select(pu =>
                                new ProgressUpdateResponse(pu.Id, pu.Date, pu.ProgressFeeling, pu.StudentId,
                                    pu.GroupId))
                            .ToList()
                    };
                }).ToList()
            };
        }
    }
}