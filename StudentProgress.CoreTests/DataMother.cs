using System.Linq;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;

namespace StudentProgress.CoreTests.UseCases
{
    public class DataMother
    {
        private DbContextOptions<ProgressContext> ContextOptions { get; }

        public DataMother(DbContextOptions<ProgressContext> contextOptions)
        {
            ContextOptions = contextOptions;
        }

        public T Query<T>() where T : class
        {
            using var context = new ProgressContext(ContextOptions);
            return context.Set<T>().FirstOrDefault();
        }

        public Group GroupWithStudents()
        {
            using var context = new ProgressContext(ContextOptions);
            return context.Groups.Include(g => g.Students).FirstOrDefault();
        }

        public Group CreateGroup(string name = "Student Group 1", string mnemonic = null,
            string[] studentNames = null)
        {
            using var context = new ProgressContext(ContextOptions);
            var group = new Group(Name.Create(name).Value, mnemonic);
            if (studentNames != null)
            {
                foreach (var studentName in studentNames)
                {
                    group.AddStudent(new Student(studentName));
                }
            }

            context.Groups.Add(group);
            context.SaveChanges();
            return @group;
        }
    }
}