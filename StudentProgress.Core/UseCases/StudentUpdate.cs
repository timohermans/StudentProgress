using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
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
            var existingStudent = await _context.Students.FirstOrDefaultAsync(s => s.Name == command.Name);
            var doesNewStudentExist = Result.SuccessIf(existingStudent == null, "Student already exists");
            var isCommandValid = Result.Combine(student, doesNewStudentExist);

            if (isCommandValid.IsFailure)
            {
                return isCommandValid;
            }

            return await student.Value
                .Update(command.Name, command.Note)
                .Tap(async () => await _context.SaveChangesAsync());
        }

        public record Command
        {
            public int Id { get; set; }
            public string Name { get; set; } = null!;
            public string? Note { get; set; } = null!;
        }
    }
}