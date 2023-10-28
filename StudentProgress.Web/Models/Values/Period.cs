using System;
using System.Collections.Generic;
using System.Linq;
using StudentProgress.Core.Extensions;
using StudentProgress.Web.Lib.Infrastructure;
using Result = CSharpFunctionalExtensions.Result;

// TODO: major refactor and cleanup required

namespace StudentProgress.Web.Models.Values
{
    public class Period
    {
        public DateTime StartDate { get; }
        public string StartDateFormattedValue => StartDate.ToString("yyyy-M-d");

        public IEnumerable<int> DaysPassedInsideSemester =>
            Enumerable.Range(0, TimePassedInsideSemesterSince(DateTime.Now).Days + 1);

        private bool IsFirstSemester => StartDate.Month.IsInRange(8, 9);
        public bool IsVeryOldDate => StartDate.Year < 1994;

        public bool IsOver
        {
            get
            {
                var today = DateTime.Today;

                if (IsFirstSemester &&
                    today.Year == StartDate.Year && today.Month.IsInRange(2, 8))
                {
                    return true;
                }

                if (!IsFirstSemester &&
                    (today.Year + 1 == StartDate.Year && today.Month < 2 ||
                     today.Year == StartDate.Year && today.Month >= 9))
                {
                    return true;
                }

                return false;
            }
        }

        public bool IsUnderway => !IsOver && !HasNotStartedYet;

        public bool HasNotStartedYet
        {
            get
            {
                var today = DateTime.Today;
                if (IsFirstSemester && today.Year < StartDate.Year || today.Month < 2)
                {
                    return true;
                }

                if (!IsFirstSemester && today.Year < StartDate.Year || today.Month < 9)
                {
                    return true;
                }

                return false;
            }
        }

        private Period(DateTime date) => StartDate = date;

        private DateTime EndOfSemester => (StartDate.Month == 2
                ? Period.Create(new DateTime(StartDate.Year, 9, 1)).Data
                : Period.Create(new DateTime(StartDate.Year + 1, 2, 1)).Data)
            .StartDate.AddDays(-1);

        public static implicit operator DateTime(Period period) => period.StartDate;
        public static explicit operator Period(DateTime date) => Create(date).Data;

        public static Result<Period> Create(DateTime date)
        {
            if (date.Month.IsInRange(2, 8))
            {
                // return CreateFirstSemesterPeriod(date);
                return new SuccessResult<Period>(new Period(new DateTime(date.Year, 2, 1)));
            }
            else
            {
                // return CreateSecondSemesterPeriod(date);
                return new SuccessResult<Period>(new Period(new DateTime(date.Year, 9, 1)));
            }
        }

        private static CSharpFunctionalExtensions.Result<Period> CreateSecondSemesterPeriod(DateTime date)
        {
            var startDate = new DateTime(date.Year, 9, 1);

            startDate = startDate.DayOfWeek switch
            {
                DayOfWeek.Sunday => startDate.AddDays(1),
                DayOfWeek.Monday => startDate,
                _ => startDate.AddDays(DayOfWeek.Monday - startDate.DayOfWeek),
            };

            return Result.Success(new Period(startDate));
        }

        private static CSharpFunctionalExtensions.Result<Period> CreateFirstSemesterPeriod(DateTime date)
        {
            var startDate = new DateTime(date.Year, 2, 1);

            startDate = startDate.DayOfWeek switch
            {
                DayOfWeek.Saturday => startDate.AddDays(2),
                DayOfWeek.Sunday => startDate.AddDays(1),
                _ => startDate
            };

            startDate = startDate.AddDays(DayOfWeek.Monday - startDate.DayOfWeek);
            startDate = startDate.AddDays(7);

            return Result.Success(new Period(startDate));
        }

        public static CSharpFunctionalExtensions.Result<Period> CreateCurrentlyActivePeriodBy(DateTime date)
        {
            var possiblePeriods = GetPossibleActivePeriods(date);
            var activePeriod = DetermineActivePeriodOutOfPossiblePeriods(date, possiblePeriods);
            if (activePeriod == null)
            {
                throw new InvalidOperationException("Unable to determine active period");
            }

            return Result.Success(activePeriod);
        }

        private static List<Period> GetPossibleActivePeriods(DateTime date)
        {
            var possiblePeriods = new List<Period>();
            var yearOfDate = date.Year == 1 ? 2 : date.Year;
            for (int year = yearOfDate - 1; year <= yearOfDate + 1; year++)
            {
                possiblePeriods.Add((Period)new DateTime(year, 2, 1));
                possiblePeriods.Add((Period)new DateTime(year, 9, 1));
            }

            return possiblePeriods;
        }

        private static Period? DetermineActivePeriodOutOfPossiblePeriods(DateTime date, List<Period> possiblePeriods)
        {
            for (int i = 1; i < possiblePeriods.Count; i++)
            {
                var minBoundary = possiblePeriods[i - 1];
                var maxBoundary = possiblePeriods[i];

                if (date >= minBoundary && date < maxBoundary)
                {
                    return minBoundary;
                }
            }

            return null;
        }

        public override string ToString() => !IsVeryOldDate
            ? IsFirstSemester
                ? $"{StartDate.Year}/{StartDate.AddYears(1).Year} - S1"
                : $"{StartDate.AddYears(-1).Year}/{StartDate.Year} - S2"
            : "No period";

        /// <summary>
        /// Gets the days that passed since the semester started.
        ///
        /// Days that are outside the semester will not be counted!
        /// </summary>
        public TimeSpan TimePassedInsideSemesterSince(DateTime sinceDate)
        {
            return sinceDate > EndOfSemester
                ? (EndOfSemester - StartDate)
                : (sinceDate - StartDate);
        }
    }
}