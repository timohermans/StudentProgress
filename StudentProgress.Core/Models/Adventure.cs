using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StudentProgress.Core.Models;

[Index(nameof(Name), nameof(DateStart), IsUnique = true)]
[Table("Adventures")]
public class Adventure
{
    public int Id { get; set; }

    [MinLength(2)]
    [MaxLength(50)]
    [DisplayName(nameof(Name))]
    public required string Name { get; set; }

    [DisplayName(nameof(Mnemonic))]
    public string? Mnemonic { get; set; }

    [DataType(DataType.Date)]
    [DisplayName(nameof(DateStart))]
    public required DateTime DateStart { get; set; }

    public ICollection<Person> People { get; set; } = new List<Person>();

    public ICollection<QuestLine> QuestLines { get; set; } = new List<QuestLine>();
}