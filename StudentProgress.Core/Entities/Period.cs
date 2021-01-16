using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using StudentProgress.Core.Extensions;

namespace StudentProgress.Core.Entities
{
    public class Period : ValueObject
    {
        public DateTime StartDate { get; }
        public string StartDateFormattedValue => StartDate.ToString("yyyy-M-d");

        private bool IsFirstSemester => StartDate.Month.IsInRange(8, 9);
        private bool IsVeryOldDate => StartDate.Year < 1994;

        private Period(DateTime date) => StartDate = date;

        public static implicit operator DateTime(Period period) => period.StartDate;
        public static explicit operator Period(DateTime date) => Create(date).Value;

        public static Result<Period> Create(DateTime date)
        {
            return date.Month switch
            {
                8 => CreateFirstSemesterPeriod(date),
                9 => CreateFirstSemesterPeriod(date),
                1 => CreateSecondSemesterPeriod(date),
                2 => CreateSecondSemesterPeriod(date),
                _ => Result.Failure<Period>("Can only create a period from February or September")
            };
        }

        private static Result<Period> CreateFirstSemesterPeriod(DateTime date)
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

        private static Result<Period> CreateSecondSemesterPeriod(DateTime date)
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

        public static Result<Period> CreateCurrentlyActivePeriodBy(DateTime date)
        {
            var possiblePeriods = GetPossibleActivePeriods(date);
            var activePeriod = DetermineActivePeriodOutOfPossiblePeriods(date, possiblePeriods);
            if (activePeriod == null)
            {
                throw new InvalidOperationException("Unable to determine active period");
            }

            return Result.Success(activePeriod);
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

        private static List<Period> GetPossibleActivePeriods(DateTime date)
        {
            var possiblePeriods = new List<Period>();
            var yearOfDate = date.Year == 1 ? 2 : date.AddYears(-1).Year;
            for (int year = yearOfDate - 1; year <= yearOfDate + 1; year++)
            {
                possiblePeriods.Add((Period) new DateTime(year, 2, 1));
                possiblePeriods.Add((Period) new DateTime(year, 9, 1));
            }

            return possiblePeriods;
        }

        public override string ToString() => !IsVeryOldDate
            ? IsFirstSemester
                ? $"{StartDate.Year}/{StartDate.AddYears(1).Year} - S1"
                : $"{StartDate.AddYears(-1).Year}/{StartDate.Year} - S2"
            : "No period";

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return StartDate;
        }
    }
}