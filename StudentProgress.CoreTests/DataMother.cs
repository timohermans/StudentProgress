using System;
using System.Collections.Generic;
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

        public StudentGroup CreateGroup(string name = "Student Group 1", DateTime? period = null,
            string mnemonic = null,
            (string LearningOutcome, string Artefact)[] milestones = null,
            params string[] studentNames)
        {
            using var context = new ProgressContext(ContextOptions);
            var group = new StudentGroup((Name) name, (Period) (period ?? new DateTime(2020, 9, 1)), mnemonic);
            if (studentNames != null)
            {
                var existingStudent = context.Students.Where(s => studentNames.Contains(s.Name)).ToList();
                var studentsToAdd = studentNames.Select(sName =>
                    existingStudent.FirstOrDefault(e => e.Name == sName) ?? new Student(sName));
                foreach (var student in studentsToAdd)
                {
                    group.AddStudent(student);
                }
            }

            if (milestones != null)
            {
                foreach (var (LearningOutcome, Artefact) in milestones)
                {
                    group.AddMilestone(new Milestone(Name.Create(LearningOutcome).Value,
                        Name.Create(Artefact).Value));
                }
            }

            context.Groups.Add(group);
            context.SaveChanges();
            return group;
        }

        public ProgressUpdate QueryProgressUpdateWithMilestonesProgress()
        {
            using var context = new ProgressContext(ContextOptions);

            return context.ProgressUpdates
                .Include(p => p.MilestonesProgress)
                .ThenInclude(p => p.Milestone)
                .FirstOrDefault();
        }

        public ProgressUpdate CreateProgressUpdate(
            StudentGroup group = null, Student student = null,
            string feedback = "bad",
            string feedup = "good",
            string feedforward = "next",
            Feeling feeling = Feeling.Neutral,
            DateTime? date = null,
            IEnumerable<MilestoneProgress> milestoneProgresses = null
        )
        {
            using var context = new ProgressContext(ContextOptions);

            if (student != null) context.Attach(student).State = EntityState.Unchanged;
            if (group != null) context.Attach(group).State = EntityState.Unchanged;

            var update = new ProgressUpdate(
                student ?? new Student("student 1"),
                group ?? new StudentGroup((Name) "group 1", (Period) new DateTime(2020, 9, 1), "mnemonic 1"),
                feedback,
                feedup,
                feedforward,
                feeling,
                date ?? new DateTime(2020, 12, 19));
            if (milestoneProgresses != null) update.AddMilestones(milestoneProgresses);
            context.ProgressUpdates.Add(update);
            context.SaveChanges();
            return update;
        }
    }
}