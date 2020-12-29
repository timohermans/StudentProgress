using System;
using System.Collections.Generic;
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

            public record StudentsResponse
            {
                public int Id { get; }
                public string Name { get; } = null!;
                [Display(Name = "Amount of Feedback")] public int AmountOfProgressItems { get; }
                [Display(Name = "Latest Feeling")] public Feeling? FeelingOfLatestProgress { get; }

                [Display(Name = "Last Time")]
                [DataType(DataType.Date)]
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
                "select exists(select 1 from \"Group\" where \"Id\" = @Id)",
                new {Id = groupId});
        }

        private async Task<Response> GetGroupWithStudentDataOfLatestFeedback(int groupId)
        {
            var groupDictionary = new Dictionary<int, Response>();
            var result = await _connection.QueryAsync<Response, Response.StudentsResponse, Response>($@"
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
	p2.""Date""
FROM ""Group"" g
LEFT JOIN ""GroupStudent"" gs ON g.""Id"" = gs.""GroupsId""
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
WHERE g.""Id"" = @Id AND 
	((p.""Date"" is null and p2.""Date"" is null) -- no progress updates
	OR 
	(p.""Date"" is not null and p2.""Date"" is not null)) -- with (aggregated) progress updates
ORDER BY p.""Date"" DESC;
", (group, studentProgress) =>
                {
                    if (!groupDictionary.TryGetValue(group.Id, out var groupEntry))
                    {
                        groupEntry = group;
                        groupEntry.Students = new List<Response.StudentsResponse>();
                        groupDictionary.Add(groupEntry.Id, groupEntry);
                    }

                    if (studentProgress != null) groupEntry.Students.Add(studentProgress);
                    return groupEntry;
                },
                splitOn: "Id",
                param: new {Id = groupId});

            return result.FirstOrDefault()!; // "!" because we did the exists check beforehand
        }
    }
}