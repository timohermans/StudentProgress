using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using StudentProgress.Core.Entities;

namespace StudentProgress.Core.UseCases
{
    public class StudentUpdate
    {
        private readonly ProgressContext _context;

        public StudentUpdate(ProgressContext context)
        {
            _context = context;
        }

        public async Task<Result> HandleAsync(Command command)
        {
            var student = Maybe<Student>.From(await _context.Students.FindAsync(command.Id))
                .ToResult("Student does not exist");

            if (student.IsFailure)
            {
                return student;
            }

            return await student.Value
                .Update(command.Note)
                .Tap(async () => await _context.SaveChangesAsync());
        }

        public record Command
        {
            public int Id { get; set; }
            public string Note { get; set; } = null!;
        }
    }
}