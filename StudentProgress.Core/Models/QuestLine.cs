using Microsoft.EntityFrameworkCore;

namespace StudentProgress.Core.Models;

[Index(nameof(Order), IsUnique = true)]
[Index(nameof(Name), IsUnique = true)]
public class QuestLine
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? MainObjective { get; set; }
    public int Order { get; set; }
    public int AdventureId { get; set; }
    public required Adventure Adventure { get; set; }
    public ICollection<Quest> Quests { get; set; } = new List<Quest>();

    public override string ToString()
    {
        return $"Adventure {AdventureId}'s {Name} ({Id})";
    }
}