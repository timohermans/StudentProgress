﻿using CSharpFunctionalExtensions;

namespace StudentProgress.CoreTests
{
    public record TestStudent(string Name, string? ExternalId = null, string? AvatarPath = null);

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
            return context.Set<T>().First();
        }

        public async Task<List<T>> QueryAllAsync<T>() where T : class
        {
            await using var context = new ProgressContext(ContextOptions);
            return await context.Set<T>().ToListAsync();
        }

        public StudentGroup GroupWithStudents()
        {
            using var context = new ProgressContext(ContextOptions);
            return context.Groups.Include(g => g.Students).First();
        }

        public StudentGroup GroupWithMilestones(int? id = null)
        {
            using var context = new ProgressContext(ContextOptions);
            var groupsWithMilestones = context.Groups.Include(g => g.Milestones);
            return id.HasValue
                ? groupsWithMilestones.First(g => g.Id == id)
                : groupsWithMilestones.First();
        }

        public ProgressTag CreateProgressTag(string name = "Tag 1")
        {
            using var context = new ProgressContext(ContextOptions);
            var tag = new ProgressTag(Name.Create(name).Value);
            context.ProgressTags.Add(tag);
            context.SaveChanges();
            return tag;
        }

        public StudentGroup CreateGroup(string name = "Student Group 1", DateTime? period = null,
            string? mnemonic = null,
            (string LearningOutcome, string Artefact)[]? milestones = null,
            params TestStudent[] students)
        {
            using var context = new ProgressContext(ContextOptions);
            var group = new StudentGroup((Name)name, (Period)(period ?? new DateTime(2020, 9, 1)), mnemonic);
            if (students.Any())
            {
                var studentNames = students.Select(s => s.Name);
                var existingStudent = context.Students.Where(s => studentNames.Contains(s.Name)).ToList();
                var studentsToAdd = students.Select(s =>
                    existingStudent.FirstOrDefault(e => e.Name == s.Name) ??
                    new Student(s.Name, s.ExternalId, s.AvatarPath));
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
                        Name.Create(Artefact).Value, group));
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
                .First();
        }

        public ProgressUpdate CreateProgressUpdate(
            StudentGroup? group = null, Student? student = null,
            string? feedback = "bad",
            Feeling feeling = Feeling.Neutral,
            DateTime? date = null,
            bool isReviewed = false,
            IEnumerable<MilestoneProgress>? milestoneProgresses = null
        )
        {
            using var context = new ProgressContext(ContextOptions);

            if (student != null) context.Attach(student).State = EntityState.Unchanged;
            if (group != null) context.Attach(group).State = EntityState.Unchanged;

            var update = new ProgressUpdate(
                student ?? new Student("student 1"),
                group ?? new StudentGroup((Name)"group 1", (Period)new DateTime(2020, 9, 1), "mnemonic 1"),
                feedback,
                feeling,
                date ?? new DateTime(2020, 12, 19),
                isReviewed);
            if (milestoneProgresses != null) update.AddMilestones(milestoneProgresses);
            context.ProgressUpdates.Add(update);
            context.SaveChanges();
            return update;
        }

        public async Task CreateSetting(string key, string value)
        {
            await using var db = new ProgressContext(ContextOptions);
            await db.Settings.AddAsync(new Setting(key, value));
            await db.SaveChangesAsync();
        }
    }
}