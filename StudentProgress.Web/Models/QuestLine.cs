namespace StudentProgress.Web.Models;

public class QuestLine
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required int Order { get; set; }
    public ICollection<Quest> Quests { get; set; }
}