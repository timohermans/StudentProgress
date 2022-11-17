namespace StudentProgress.Core.Entities
{
    public class MilestoneProgress : AuditableEntity<int>
    {
        public Milestone Milestone { get; private set; }
        public Rating Rating { get; private set; }
        public string? Comment { get; private set; }
        public int MilestoneId { get; private set; }
        public int ProgressUpdateId { get; private set; }

#nullable disable
        public ProgressUpdate ProgressUpdate { get; private set; }
        private MilestoneProgress() { }
#nullable enable

        public MilestoneProgress(Rating rating, Milestone milestone, string? comment)
        {
            Rating = rating;
            Milestone = milestone;
            Comment = comment;
        }

        public void Update(string? comment, Rating rating)
        {
            Comment = comment;
            Rating = rating;
        }
    }
}
