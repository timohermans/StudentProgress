namespace StudentProgress.Core.Models;

public class Objective
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTime OptimalTargetDeadLine { get; set; }
    public required string Color { get; set; }
    public ICollection<ObjectiveProgress> Progresses { get; set; } = new List<ObjectiveProgress>();

    public string ColorProgressBorder => $"{Color}88";
    public string ColorProgressBackground => $"{Color}88";
    public string ColorProgressAddBackground => $"{Color}22";
}