using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using StudentProgress.Core.Entities;

namespace StudentProgress.Core.UseCases
{
    public class StudentGroupUpdate
    {
        public class Request
        {
            public int Id { get; set; }
            [Required]
            public string Name { get; set; }
            public string Mnemonic { get; set; }
        }

        private readonly ProgressContext _context;

        public StudentGroupUpdate(ProgressContext context)
        {
            _context = context;
        }

        public async Task HandleAsync(Request request)
        {
            var studentGroup = (await _context.StudentGroup.FindAsync(request.Id)) ?? throw new System.NullReferenceException(nameof(StudentGroup));

            studentGroup.UpdateGroup(request.Name, request.Mnemonic);
            await _context.SaveChangesAsync();
        }
    }
}