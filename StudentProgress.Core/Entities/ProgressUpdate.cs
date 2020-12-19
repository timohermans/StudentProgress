using System;

namespace StudentProgress.Core.Entities
{
    public class ProgressUpdate : AuditableEntity<int>
    {
        public Student Student { get; private set; }
        public Group Group { get; private set; }
        public string? Feedback { get; private set; }
        public string? Feedup { get; private set; }
        public string? Feedforward { get; private set; }
        public Feeling ProgressFeeling { get; private set; }
        public int GroupId { get; private set; }
        public int StudentId { get; private set; }

        public DateTime Date { get; private set; }

        #nullable disable
        private ProgressUpdate() { }
        #nullable enable

        public ProgressUpdate(Student student, Group group, string? feedback, string? feedup, string? feedforward, Feeling progressFeeling, DateTime date)
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

        public void Update(Feeling feeling, DateTime date, string? feedback, string? feedup, string? feedforward)
        {
            ProgressFeeling = feeling;
            Feedback = feedback;
            Feedup = feedup;
            Feedforward = feedforward;
            Date = date;
        }
    }
}
