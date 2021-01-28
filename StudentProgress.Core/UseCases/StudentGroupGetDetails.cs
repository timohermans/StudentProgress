using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using StudentProgress.Core.Entities;
using Dapper;

namespace StudentProgress.Core.UseCases
{
    public class StudentGroupGetDetails
    {
        private readonly IDbConnection _connection;

        public StudentGroupGetDetails(IDbConnection connection)
        {
            _connection = connection;
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

            public record MilestoneResponse
            {
                public int Id { get; }
                public string Artefact { get; } = null!;
                [DisplayName("Learning Outcome")]
                public string LearningOutcome { get; } = null!;
            }

            public record StudentsResponse
            {
                public int Id { get; }
                public string Name { get; } = null!;
                [Display(Name = "Amount of Feedback")] public int AmountOfProgressItems { get; }
                [Display(Name = "Latest Feeling")] public Feeling? FeelingOfLatestProgress { get; }

                [Display(Name = "Last Time")]
                [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
                public DateTime? LastUpdateDate { get; }

                [Display(Name = "Last Feedforward")] public string? LastFeedforward { get; }
            }
        }

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
                await _connection.QueryAsync<Response, Response.StudentsResponse, Response.MilestoneResponse, Response>(
                    $@"
SELECT
    g.""Id"",
    g.""Name"",
    g.""Mnemonic"",
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
ORDER BY p.""Date"" DESC, m.""LearningOutcome"", m.""Artefact"";
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
    }
}