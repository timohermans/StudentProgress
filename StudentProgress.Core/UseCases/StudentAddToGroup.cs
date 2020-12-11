using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentProgress.Core.UseCases
{
    public class StudentAddToGroup
    {
        private readonly ProgressContext context;

        public StudentAddToGroup(ProgressContext context)
        {
            this.context = context;
        }

        public record Request
        {
            [Required]
            public string Name { get; init; } = null!;
            [Required]
            public int GroupId { get; init; }
        };

        public async Task<bool> HandleAsync(Request request)
        {
            var studentGroup = context.StudentGroup.Include(_ => _.Students).FirstOrDefault(group => group.Id == request.GroupId);

            if (studentGroup == null)
            {
                throw new InvalidOperationException("Group not found");
            }

            var student = context.Student.FirstOrDefault(s => s.Name == request.Name);

            if (student == null)
            {
                student = new Student(request.Name);
                await context.Student.AddAsync(student);
            }

            studentGroup.AddStudent(student);

            await context.SaveChangesAsync();

            return true;
        }
    }
}
