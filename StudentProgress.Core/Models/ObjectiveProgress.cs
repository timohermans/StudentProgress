namespace StudentProgress.Core.Models;

public class ObjectiveProgress
{
    public int Id { get; set; }
    public required DateTime AchievedAt { get; set; }
    public required Person Person { get; set; }
}