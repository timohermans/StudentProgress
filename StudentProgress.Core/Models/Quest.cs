namespace StudentProgress.Core.Models;

public class Quest
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? MainObjective { get; set; }
    public string? Description { get; set; }
    public ICollection<Objective> Objectives { get; set; } = new List<Objective>();
}