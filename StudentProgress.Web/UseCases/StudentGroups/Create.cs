using StudentProgress.Web.Data;
using StudentProgress.Web.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace StudentProgress.Web.UseCases.StudentGroups
{
    public class Create
    {
        private ProgressContext context;

        public Create(ProgressContext context)
        {
            this.context = context;
        }

        public record Request
        {
            [Required]
            public string Name { get; init; }
        }


        public async Task HandleAsync(Request request)
        {
            await context.StudentGroup.AddAsync(new StudentGroup(request.Name));
            await context.SaveChangesAsync();
        }
    }
}
