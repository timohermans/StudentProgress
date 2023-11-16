namespace StudentProgress.Core.Models;

public class Quest
{
    public int Id { get; set; }
    public required string ObjectiveMain { get; set; }
    public required string Description { get; set; }
    public ICollection<Objective> Objectives { get; set; } = new List<Objective>();

}