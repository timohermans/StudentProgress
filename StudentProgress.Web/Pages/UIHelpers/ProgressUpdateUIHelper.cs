using System;
using System.Collections.Generic;
using System.Linq;
using StudentProgress.Core.Entities;

namespace StudentProgress.Web.Pages.UIHelpers
{
    public record ProgressTimeline(Period Period, IEnumerable<ProgressUpdateUI> Updates);
    public record ProgressUpdateUI(int Id, DateTime Date, Feeling Feeling, int StudentId, int GroupId);

    public class ProgressUpdateUIHelper
    {
        public static IDictionary<int, ProgressUpdateUI?> GetTimelineWithUpdatesFrom(Period period,
            IEnumerable<ProgressUpdateUI> updates)
        {
            return period.DaysPassedInsideSemester
                        .ToDictionary(day => day, day => updates.FirstOrDefault(u => u.Date.Date == period.StartDate.AddDays(day).Date));
        }
        
        public static string GetTimelineBgColor(Period period, int dayFromStart, ProgressUpdateUI? update)
        {
            var date = period.StartDate.AddDays(dayFromStart);

            var borderClass = date.DayOfWeek switch {
                DayOfWeek.Monday => "timeline-week-border",
                _ => ""
            };

            if (update == null)
            {
                return $"{borderClass} bg-white";
            }

            var colorClass = update.Feeling switch
            {
                Feeling.Bad => "bg-danger",
                Feeling.Neutral => "bg-warning",
                Feeling.Good => "bg-success",
                _ => ""
            };

            return $"{colorClass} {borderClass} timeline-item-hoverable";
        }

        public static string ToFeelingBackgroundColor(Feeling feeling)
        {
            return feeling switch
            {
                Feeling.Bad => "bg-danger",
                Feeling.Neutral => "bg-warning",
                Feeling.Good => "bg-success",
                _ => ""
            };
        }
    }
}