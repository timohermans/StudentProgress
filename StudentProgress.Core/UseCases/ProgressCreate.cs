using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
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

        public record Request
        {
            [Required]
            public int StudentId { get; set; }
            [Required]
            public int GroupId { get; set; }
            [Required]
            public Feeling Feeling { get; set; }
            [Required]
            [DataType(DataType.Date)]
            public DateTime Date { get; set; }
            public string? Feedback { get; set; }
            public string? Feedup { get; set; }
            public string? Feedforward { get; set; }
        }

        public async Task HandleAsync(Request progress)
        {
            var student = context.Student.FirstOrDefault(s => s.Id == progress.StudentId);
            var group = context.StudentGroup.FirstOrDefault(g => g.Id == progress.GroupId);

            if (student == null || group == null)
            {
                throw new InvalidOperationException("Either student or group doesn't exist (anymore)");
            }

            await context.ProgressUpdate.AddAsync(new Entities.ProgressUpdate(
                student,
                group,
                progress.Feedback,
                progress.Feedup,
                progress.Feedforward,
                progress.Feeling,
                progress.Date));
            await context.SaveChangesAsync();
        }
    }
}
