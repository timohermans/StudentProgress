using Microsoft.EntityFrameworkCore;
using StudentProgress.Web.Data;
using StudentProgress.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentProgress.Web.UseCases.Progress
{
    public class GetProgressForStudentInGroup
    {
        public record Request(int? GroupId, int? StudentId);
        public record Response
        {
            public int StudentId { get; init; }
            public int GroupId { get; init; }
            public string Name { get; init; }
            public string GroupName { get; init; }
            public IEnumerable<ProgressUpdateResponse> ProgressUpdates { get; init; }

            public record ProgressUpdateResponse
            {
                public int Id { get; init; }
                public string Feedback { get; init; }
                public string Feedup { get; init; }
                public string Feedforward { get; init; }
                public DateTime UpdatedAt { get; init; }
                public DateTime CreatedAt { get; init; }
                [DataType(DataType.Date)]
                public DateTime Date { get; init; }
                public Feeling Feeling { get; init; }
            }
        }

        private readonly ProgressContext _context;
        public GetProgressForStudentInGroup(ProgressContext context)
        {
            _context = context;
        }

        public async Task<Response> HandleAsync(Request request)
        {
            var student = await _context.Student.Include(_ => _.ProgressUpdates).FirstOrDefaultAsync(g => g.Id == (request.StudentId ?? 0));
            var group = _context.StudentGroup.FirstOrDefault(g => g.Id == (request.GroupId ?? 0));

            if (student == null || group == null) throw new InvalidOperationException("Must supply a valid student and group");

            var progressForGroup = student.ProgressUpdates
                .Where(p => p.GroupId == request.GroupId)
                .OrderByDescending(p => p.CreatedDate);

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
                    Feedforward = p.Feedforward,
                    Feedup = p.Feedup,
                    Feeling = p.ProgressFeeling,
                    Date = p.Date,
                    UpdatedAt = p.UpdatedDate,
                    CreatedAt = p.CreatedDate
                })
            };
        }
    }
}
