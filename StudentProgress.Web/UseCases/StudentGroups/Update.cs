using StudentProgress.Web.Data;
using StudentProgress.Web.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace StudentProgress.Web.UseCases.StudentGroups
{
    public class Update
    {
        public class Request
        {
            public int Id { get; set; }
            [Required]
            public string Name { get; set; }
            public string Mnemonic { get; set; }
        }

        private readonly ProgressContext _context;

        public Update(ProgressContext context)
        {
            _context = context;
        }

        public async Task HandleAsync(Request request)
        {
            var studentGroup = (await _context.StudentGroup.FindAsync(request.Id)) ?? throw new NullReferenceException(nameof(StudentGroup));

            studentGroup.UpdateGroup(request.Name, request.Mnemonic);
            await _context.SaveChangesAsync();
        }
    }
}
