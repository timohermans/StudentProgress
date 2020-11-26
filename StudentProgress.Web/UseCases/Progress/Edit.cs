using StudentProgress.Web.Data;
using StudentProgress.Web.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentProgress.Web.UseCases.Progress
{
    public class Edit
    {
        private readonly ProgressContext context;

        public Edit(ProgressContext context)
        {
            this.context = context;
        }

        public record Request
        {
            [Required]
            public int Id { get; init; }
            [Required]
            public Feeling Feeling { get; init; }
            [Required]
            [DataType(DataType.Date)]
            public DateTime Date { get; init; }
            public string Feedback { get; init; }
            public string Feedup { get; init; }
            public string Feedforward { get; init; }
        }

        public async Task HandleAsync(Request request)
        {
            var progress = await context.ProgressUpdate.FindAsync(request.Id);

            if (progress == null)
            {
                throw new InvalidOperationException("Either student or group doesn't exist (anymore)");
            }

            progress.Update(request.Feeling, request.Date, request.Feedback, request.Feedup, request.Feedforward);

            await context.SaveChangesAsync();
        }
    }
}
