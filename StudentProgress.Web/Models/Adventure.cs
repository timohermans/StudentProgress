using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentProgress.Web.Models;

public class Adventure
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Mnemonic { get; set; }
    [DataType(DataType.Date)] public required DateTime DateStart { get; set; }
    public List<Person> People { get; set; } = new();
}