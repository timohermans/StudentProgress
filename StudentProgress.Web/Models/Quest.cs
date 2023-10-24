namespace StudentProgress.Web.Models;

public class Quest
{
    public int Id { get; set; }
    public string ObjectiveMain { get; set; }
    public string Description { get; set; }
    public ICollection<Objective> Objectives { get; set; }
    
}