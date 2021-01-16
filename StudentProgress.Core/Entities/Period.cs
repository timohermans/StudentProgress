using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using StudentProgress.Core.Extensions;

namespace StudentProgress.Core.Entities
{
    public class Period : ValueObject
    {
        public DateTime StartDate { get; }

        private bool IsFirstSemester => StartDate.Month.IsInRange(8, 9);

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

        public override string ToString() => StartDate > new DateTime(1990, 1, 1)
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