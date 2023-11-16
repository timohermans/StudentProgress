using StudentProgress.Core.Extensions;
using StudentProgress.Core.Infrastructure;


// TODO: major refactor and cleanup required

namespace StudentProgress.Core.Models.Values;

public record Period
{
    public DateTime StartDate { get; init; }

    public static implicit operator DateTime(Period period) => period.StartDate;

    public static explicit operator Period(DateTime date) => new()
    {
        StartDate = date
    };

    public bool IsOver(IDateProvider dp)
    {
        DateTime today = dp.Today();

        DateTime EndDate(DateTime date) => IsInFirstHalfOfYear(date)
            ? new DateTime(date.Year, 7, 1).AddDays(-1)
            : new DateTime(date.Year + 1, 2, 1).AddDays(-1);

        return today > EndDate(StartDate);
    }

    public bool HasNotStartedYet(IDateProvider dp) => dp.Today() < StartDate;

    public bool IsUnderway(IDateProvider dp) => !IsOver(dp) && !HasNotStartedYet(dp);

    bool IsInFirstHalfOfYear(DateTime date) => date.Month.IsInRange(1, 6);

    public override string ToString()
    {
        string ToShortYear(int year) => year.ToString().Substring(2, 2);

        var year = StartDate.Year;
        var previous = ToShortYear(year - 1);
        var current = ToShortYear(year);
        var next = ToShortYear(year + 1);

        return IsInFirstHalfOfYear(StartDate)
            ? $"{previous}{current}vj"
            : $"{current}{next}nj";
    }
}