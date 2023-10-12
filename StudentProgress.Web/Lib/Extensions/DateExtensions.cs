using System;
using StudentProgress.Web.Lib.Infrastructure;

namespace StudentProgress.Web.Lib.Extensions;

public static class DateExtensions
{
    public static string TimePassed(this DateTime date, IDateProvider dateProvider)
    {
        var timePassed = dateProvider.Now() - date;

        if (timePassed.Days > 1) return $"{timePassed.Days} days";
        if (timePassed.Days == 1) return $"{timePassed.Days} day";
        if (timePassed.Hours > 1) return $"{timePassed.Hours} hours";
        if (timePassed.Hours == 1) return $"{timePassed.Hours} hour";
        if (timePassed.Minutes > 1) return $"{timePassed.Minutes} minutes";
        if (timePassed.Minutes == 1) return $"{timePassed.Minutes} minute";
        if (timePassed.Seconds > 1) return $"{timePassed.Seconds} seconds";
        return "Just now";
    }

    public static string TimePassedShort(this DateTime date, IDateProvider dateProvider)
    {
        var timePassed = dateProvider.Now() - date;

        if (timePassed.Days > 1) return $"{timePassed.Days}d";
        if (timePassed.Days == 1) return $"{timePassed.Days}d";
        if (timePassed.Hours > 1) return $"{timePassed.Hours}h";
        if (timePassed.Hours == 1) return $"{timePassed.Hours}h";
        if (timePassed.Minutes > 1) return $"{timePassed.Minutes}m";
        if (timePassed.Minutes == 1) return $"{timePassed.Minutes}m";
        if (timePassed.Seconds > 1) return $"{timePassed.Seconds}s";
        return "Now";
    }
}