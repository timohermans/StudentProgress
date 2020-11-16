using Microsoft.EntityFrameworkCore;
using StudentProgress.Web.Data;
using StudentProgress.Web.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentProgress.Web.UseCases.Students
{
    public class AddToGroup
    {
        private ProgressContext context;

        public AddToGroup(ProgressContext context)
        {
            this.context = context;
        }

        public record Request
        {
            [Required]
            public string Name { get; init; }
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
