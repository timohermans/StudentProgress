using System;
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
            return group;
        }

        public ProgressUpdate CreateProgressUpdate(
            string groupName = "group 1", string studentName = "student 1",
            string feedback = "This is not so good",
            string feedup = "This is looking good",
            string feedforward = "Work on this",
            Feeling feeling = Feeling.Neutral,
            DateTime? date = null
        )
        {
            using var context = new ProgressContext(ContextOptions);
            var student = context.Students.FirstOrDefault(s => s.Name == studentName) ?? new Student(studentName);
            var group = context.Groups.FirstOrDefault(g => g.Name == groupName) ??
                        new Group(Name.Create(groupName).Value, null);

            var update = new ProgressUpdate(
                student,
                group,
                feedback,
                feedup,
                feedforward,
                feeling,
                date ?? new DateTime(2020, 12, 19));
            context.ProgressUpdates.Add(update);
            context.SaveChanges();
            return update;
        }
    }
}