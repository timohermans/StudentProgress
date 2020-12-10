using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;

namespace StudentProgress.Core.UseCases
{
    public class StudentGroupGetDetails
    {
        private readonly ProgressContext context;

        public StudentGroupGetDetails(ProgressContext context)
        {
            this.context = context;
        }

        public record Request(int Id);

        public record Response
        {
            public int Id { get; init; }
            public string Name { get; init; }
            public string Mnemonic { get; init; }
            [Display(Name = "Created On")] public DateTime CreatedAt { get; init; }
            [Display(Name = "Last name change")] public DateTime UpdatedAt { get; init; }
            public IEnumerable<StudentsResponse> Students { get; init; }

            public record StudentsResponse
            {
                public int Id { get; }
                public string Name { get; }
                [Display(Name = "Amount of Feedback")] public int AmountOfProgressItems { get; }
                [Display(Name = "Latest Feeling")] public Feeling? FeelingOfLatestProgress { get; }

                [Display(Name = "Last Time")]
                [DataType(DataType.Date)]
                public DateTime? LastUpdateDate { get; }

                [Display(Name = "Last Feedforward")] public string LastFeedforward { get; }

                public StudentsResponse(int id, string name, int amountOfProgressItems,
                    Feeling? feelingOfLatestProgress, DateTime? lastUpdateDate, string lastFeedforward)
                {
                    Id = id;
                    Name = name;
                    AmountOfProgressItems = amountOfProgressItems;
                    FeelingOfLatestProgress = feelingOfLatestProgress;
                    LastFeedforward = lastFeedforward;
                    LastUpdateDate = lastUpdateDate;
                }
            }
        }

        public async Task<Response> HandleAsync(Request request)
        {
            var studentGroup = await context.StudentGroup.FirstOrDefaultAsync(g => g.Id == request.Id);

            if (studentGroup == null)
            {
                return null;
            }

            var students = context.Student.Include(_ => _.ProgressUpdates)
                .Where(s => s.StudentGroups.Any(g => g.Id == request.Id))
                .Select(student => new Response.StudentsResponse(
                    student.Id,
                    student.Name,
                    student.ProgressUpdates.Count(p => p.Group.Id == request.Id),
                    student.ProgressUpdates.OrderByDescending(p => p.Date).FirstOrDefault().ProgressFeeling,
                    student.ProgressUpdates.OrderByDescending(p => p.Date).FirstOrDefault().Date,
                    student.ProgressUpdates.OrderByDescending(p => p.Date)
                        .FirstOrDefault(_ => _.Group.Id == request.Id && !string.IsNullOrEmpty(_.Feedforward))
                        .Feedforward
                ));

            return new Response
            {
                Id = studentGroup.Id,
                Name = studentGroup.Name,
                Mnemonic = studentGroup.Mnemonic,
                CreatedAt = studentGroup.CreatedDate,
                UpdatedAt = studentGroup.UpdatedDate,
                Students = students
            };
        }
    }
}