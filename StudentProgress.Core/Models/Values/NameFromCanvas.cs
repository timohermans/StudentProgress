using System.Linq;
using StudentProgress.Core.Infrastructure;

namespace StudentProgress.Core.Models.Values;

public class NameFromCanvas
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Initials { get; set; }

    public static Result<NameFromCanvas> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new SuccessResult<NameFromCanvas>(
                new NameFromCanvas
                {
                    FirstName = "Unknown",
                    LastName = "Unknown"
                });
        }

        var nameSplits = value.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(n => n.Trim()).ToList();

        if (nameSplits.Count < 2)
        {
            return new ErrorResult<NameFromCanvas>(
                "This string does not contain at least two parts seperated by a comma");
        }

        var lastName = nameSplits.First();

        nameSplits = nameSplits.Last()
            .Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();

        string? initials = null;
        if (nameSplits.Last().Contains('.'))
        {
            initials = nameSplits.Last();
            nameSplits.RemoveAt(nameSplits.Count - 1);
        }

        var firstName = string.Join(' ', nameSplits);

        return new SuccessResult<NameFromCanvas>(new NameFromCanvas
        {
            FirstName = firstName,
            LastName = lastName,
            Initials = initials
        });
    }
}