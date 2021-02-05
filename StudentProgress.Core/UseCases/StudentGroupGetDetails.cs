using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;

namespace StudentProgress.Core.UseCases
{
    public class StudentGroupGetDetails
    {
        private readonly IDbConnection _connection;
        private readonly ProgressContext _context;

        public StudentGroupGetDetails(IDbConnection connection, ProgressContext context)
        {
            _connection = connection;
            _context = context;
        }

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
            public Period Period { get; set; }

            public record MilestoneResponse
            {
                public int Id { get; }

                [DisplayName("Learning Outcome Artefact")]
                public string Artefact { get; } = null!;

                [DisplayName("Learning Outcome")] public string LearningOutcome { get; } = null!;
            }

            public record StudentsResponse
            {
                public int Id { get; }
                public string Name { get; } = null!;
                [Display(Name = "#")] public int AmountOfProgressItems { get; }
                [Display(Name = "Latest Feeling")] public Feeling? FeelingOfLatestProgress { get; }

                [Display(Name = "Last Time")]
                [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
                public DateTime? LastUpdateDate { get; }

                [Display(Name = "Last Feedforward")] public string? LastFeedforward { get; }

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

            var group = await GetGroupWithStudentDataOfLatestFeedback(request.Id);
            group.Students = await GetStudentsWithProgressUpdates(group);
            return group;
        }

        private async Task<bool> DoesGroupExist(int groupId)
        {
            return await _connection.QueryFirstAsync<bool>(
                "select exists(select 1 from \"StudentGroup\" where \"Id\" = @Id)",
                new {Id = groupId});
        }

        private async Task<Response> GetGroupWithStudentDataOfLatestFeedback(int groupId)
        {
            var groupDictionary = new Dictionary<int, Response>();
            var studentProgressDictionary = new Dictionary<int, Response.StudentsResponse>();
            var milestoneDictionary = new Dictionary<int, Response.MilestoneResponse>();
            var result =
                await _connection
                    .QueryAsync<Response, Response.StudentsResponse, Response.MilestoneResponse, Response>(
                        $@"
SELECT
    g.""Id"",
    g.""Name"",
    g.""Mnemonic"",
    g.""Period"",
    g.""CreatedDate"" as ""{nameof(Response.CreatedAt)}"",
    g.""UpdatedDate"" as ""{nameof(Response.UpdatedAt)}"",
    s.""Id"",
    s.""Name"",
    p.""Date"" as ""{nameof(Response.StudentsResponse.LastUpdateDate)}"",
    p.""ProgressFeeling"" as ""{nameof(Response.StudentsResponse.FeelingOfLatestProgress)}"",
    p.""Feedforward"" as ""{nameof(Response.StudentsResponse.LastFeedforward)}"",
    p2.""AmountOfProgressItems"" as ""{nameof(Response.StudentsResponse.AmountOfProgressItems)}"",
	p.""Date"",
	p2.""Date"",
    m.""Id"",
    m.""Artefact"",
    m.""LearningOutcome""
FROM ""StudentGroup"" g
LEFT JOIN ""StudentStudentGroup"" gs ON g.""Id"" = gs.""StudentGroupsId""
LEFT JOIN ""Student"" s ON s.""Id"" = gs.""StudentsId""
LEFT JOIN ""ProgressUpdate"" p ON p.""GroupId"" = g.""Id"" AND p.""StudentId"" = s.""Id""
LEFT JOIN
		(select 
			""StudentId"",
            ""GroupId"",
			MAX(""Date"") as ""Date"", 
			COUNT(*) as ""AmountOfProgressItems""
		FROM ""ProgressUpdate""
		group by ""GroupId"", ""StudentId"") p2
    ON p.""StudentId"" = p2.""StudentId""
    AND p.""GroupId"" = p2.""GroupId""
    AND p.""Date"" = p2.""Date""
LEFT JOIN ""Milestone"" m ON g.""Id"" = m.""StudentGroupId""
WHERE g.""Id"" = @Id AND 
	((p.""Date"" is null and p2.""Date"" is null) -- no progress updates
	OR 
	(p.""Date"" is not null and p2.""Date"" is not null)) -- with (aggregated) progress updates
ORDER BY s.""Name"", p.""Date"" DESC, m.""LearningOutcome"", m.""Artefact"";
", (group, studentProgress, milestone) =>
                        {
                            if (!groupDictionary.TryGetValue(group.Id, out var groupEntry))
                            {
                                groupEntry = group;
                                groupEntry.Students = new List<Response.StudentsResponse>();
                                groupEntry.Milestones = new List<Response.MilestoneResponse>();
                                groupDictionary.Add(groupEntry.Id, groupEntry);
                            }

                            if (studentProgress != null && !studentProgressDictionary.ContainsKey(studentProgress.Id))
                            {
                                groupEntry.Students.Add(studentProgress);
                                studentProgressDictionary.Add(studentProgress.Id, studentProgress);
                            }

                            if (milestone != null && !milestoneDictionary.ContainsKey(milestone.Id))
                            {
                                groupEntry.Milestones.Add(milestone);
                                milestoneDictionary.Add(milestone.Id, milestone);
                            }

                            return groupEntry;
                        },
                        splitOn: "Id",
                        param: new {Id = groupId});

            return result.FirstOrDefault()!; // "!" because we did the exists check beforehand
        }

        private async Task<IList<Response.StudentsResponse>> GetStudentsWithProgressUpdates(Response group)
        {
            var studentIds = group.Students.Select(s => s.Id).ToList();
            var progressUpdates = await _context.ProgressUpdates
                    .Where(p => studentIds.Contains(p.StudentId) && p.GroupId == group.Id)
                    .Select(p => new ProgressUpdateResponse(p.Id, p.Date, p.ProgressFeeling, p.StudentId, p.GroupId))
                    .ToListAsync();
            return group.Students.Select(s => s with
            {
                ProgressUpdates = s.ProgressUpdates = progressUpdates.Where(p => p.StudentId == s.Id).ToList()
            }).ToList();
        }
    }
}