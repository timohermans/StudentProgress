using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace StudentProgress.Core.Entities
{
    public class ProgressUpdate : AuditableEntity<int>
    {
        public Student Student { get; private set; }
        public StudentGroup Group { get; private set; }
        public string? Feedback { get; private set; }
        public string? Feedup { get; private set; }
        public string? Feedforward { get; private set; }
        public Feeling ProgressFeeling { get; private set; }
        public int GroupId { get; private set; }
        public int StudentId { get; private set; }
        public DateTime Date { get; private set; }
        private readonly List<MilestoneProgress> _milestonesProgress = new List<MilestoneProgress>();
        public IReadOnlyList<MilestoneProgress> MilestonesProgress => _milestonesProgress;

        #nullable disable
        private ProgressUpdate() { }
        #nullable enable

        public ProgressUpdate(Student student, StudentGroup group, string? feedback, string? feedup, string? feedforward, Feeling progressFeeling, DateTime date)
        {
            Student = student ?? throw new NullReferenceException(nameof(student));
            Group = group ?? throw new NullReferenceException(nameof(group));
            Feedback = feedback;
            Feedup = feedup;
            Feedforward = feedforward;
            ProgressFeeling = progressFeeling;
            GroupId = group.Id;
            Date = date;
        }

        public Result Update(Feeling feeling, DateTime date, string? feedback, string? feedup, string? feedforward)
        {
            ProgressFeeling = feeling;
            Feedback = feedback;
            Feedup = feedup;
            Feedforward = feedforward;
            Date = date;

            return Result.Success();
        }

        public Result AddMilestones(IEnumerable<MilestoneProgress> milestones)
        {
            _milestonesProgress.Clear();
            _milestonesProgress.AddRange(milestones);
            return Result.Success();
        }
    }
}
