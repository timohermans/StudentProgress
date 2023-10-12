using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace StudentProgress.Web.Models;

[Index(nameof(Name), nameof(DateStart), IsUnique = true)]
public class Adventure
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Mnemonic { get; set; }
    [DataType(DataType.Date)] public required DateTime DateStart { get; set; }
    public ICollection<Person> People { get; } = new List<Person>();
}