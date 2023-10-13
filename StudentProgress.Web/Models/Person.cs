using System.Collections.Generic;
using StudentProgress.Core.Entities;

namespace StudentProgress.Web.Models;

public class Person
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? AvatarPath { get; set; }
    public string? ExternalId { get; set; }
    // public IEnumerable<ProgressUpdate> ProgressUpdates { get; set; }
    // public IEnumerable<StudentGroup> StudentGroups { get; set; }
    public string? Note { get; set; }
    public ICollection<Adventure> Adventures { get; set; } = new List<Adventure>();
}