namespace StudentProgress.Web.Models;

public class Person
{
    public int Id { get; set; }
    public string Name => FirstName + ", " + LastName + " " + Initials;
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Initials { get; set; }
    public string? AvatarPath { get; set; }

    public string ImageSource => string.IsNullOrEmpty(AvatarPath)
        ? "/images/avatar-placeholder.png"
        : $"/media/{AvatarPath}";

    public string? ExternalId { get; set; }

    // public IEnumerable<ProgressUpdate> ProgressUpdates { get; set; }
    // public IEnumerable<StudentGroup> StudentGroups { get; set; }
    public string? Note { get; set; }
    public ICollection<Adventure> Adventures { get; set; } = new List<Adventure>();
}