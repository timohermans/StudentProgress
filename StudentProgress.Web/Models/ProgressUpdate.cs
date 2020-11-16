using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentProgress.Web.Models
{
    public class ProgressUpdate : AuditableEntity
    {
        public int Id { get; private set; }
        public Student Student { get; private set; }
        public StudentGroup Group { get; private set; }
        public string Feedback { get; private set; }
        public string Feedup { get; private set; }
        public string Feedforward { get; private set; }
        public Feeling ProgressFeeling { get; private set; }
        public int GroupId { get; private set; }
        
        public DateTime Date { get; private set; }

        private ProgressUpdate() { }

        public ProgressUpdate(Student student, StudentGroup group, string feedback, string feedup, string feedforward, Feeling progressFeeling, DateTime date)
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

        public void UpdateProgress(Feeling feeling, string feedback, string feedforward, string feedup)
        {
            // TODO: Create ProgressUpdateHistory item
            // Add Reason
            // return the history object
            ProgressFeeling = feeling;
            Feedback = feedback;
            Feedup = feedup;
            Feedforward = feedforward;
        }
    }
}
