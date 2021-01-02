using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;

namespace StudentProgress.CoreTests
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

        public StudentGroup GroupWithStudents()
        {
            using var context = new ProgressContext(ContextOptions);
            return context.Groups.Include(g => g.Students).FirstOrDefault();
        }

        public StudentGroup GroupWithMilestones()
        {
            using var context = new ProgressContext(ContextOptions);
            return context.Groups.Include(g => g.Milestones).FirstOrDefault();
        }

        public StudentGroup CreateGroup(string name = "Student Group 1", string mnemonic = null,
            string[] milestoneNames = null,
            params string[] studentNames)
        {
            using var context = new ProgressContext(ContextOptions);
            var group = new StudentGroup(Name.Create(name).Value, mnemonic);
            if (studentNames != null)
            {
                foreach (var studentName in studentNames)
                {
                    group.AddStudent(new Student(studentName));
                }
            }

            if (milestoneNames != null)
            {
                foreach (var milestoneName in milestoneNames)
                {
                    group.AddMilestone(new Milestone(Name.Create(milestoneName).Value));
                }
            }

            context.Groups.Add(group);
            context.SaveChanges();
            return group;
        }

        public ProgressUpdate CreateProgressUpdate(
            StudentGroup group = null, Student student = null,
            string feedback = "This is not so good",
            string feedup = "This is looking good",
            string feedforward = "Work on this",
            Feeling feeling = Feeling.Neutral,
            DateTime? date = null
        )
        {
            using var context = new ProgressContext(ContextOptions);

            if (student != null) context.Attach(student).State = EntityState.Unchanged;
            if (group != null) context.Attach(group).State = EntityState.Unchanged;

            var update = new ProgressUpdate(
                student ?? new Student("student 1"),
                group ?? new StudentGroup(Name.Create("group 1").Value, "mnemonic 1"),
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