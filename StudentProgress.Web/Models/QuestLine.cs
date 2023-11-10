using Microsoft.EntityFrameworkCore;

namespace StudentProgress.Web.Models;

[Index(nameof(Order), IsUnique = true)]
[Index(nameof(Name), IsUnique = true)]
public class QuestLine
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Order { get; set; }
    public ICollection<Quest> Quests { get; set; } = new List<Quest>();
}