using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace StudentProgress.Web.Models;

[Index(nameof(Email), IsUnique = true)]
public class User
{
    public required string Email { get; set; }
    [DataType(DataType.Password)] public required string Password { get; set; }
}