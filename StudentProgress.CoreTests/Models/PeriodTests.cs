using System;
using FluentAssertions;
using StudentProgress.Core.Entities;
using Xunit;

namespace StudentProgress.CoreTests.Models
{
    public class PeriodTests
    {
        [Theory]
        [InlineData(2019, 9, 1, 2019, 9, 2)]
        [InlineData(2020, 9, 1, 2020, 8, 31)]
        [InlineData(2021, 9, 1, 2021, 8, 30)]
        [InlineData(2021, 8, 30, 2021, 8, 30)]
        public void First_semesters_always_starts_on_the_first_week_of_september(int createYear, int createMonth, int createDay,
            int expectedYear, int expectedMonth, int expectedDay)
        {
            var period = Period.Create(new DateTime(createYear, createMonth, createDay));

            period.IsSuccess.Should().BeTrue();
            period.Value.StartDate.Should().Be(new DateTime(expectedYear, expectedMonth, expectedDay));
        }
        
        [Theory]
        [InlineData(2021, 2, 1, 2021, 2, 8)]
        [InlineData(2020, 2, 1, 2020, 2, 10)]
        public void Second_semesters_always_starts_on_the_second_week_of_february(int createYear, int createMonth, int createDay,
            int expectedYear, int expectedMonth, int expectedDay)
        {
            var period = Period.Create(new DateTime(createYear, createMonth, createDay));

            period.IsSuccess.Should().BeTrue();
            period.Value.StartDate.Should().Be(new DateTime(expectedYear, expectedMonth, expectedDay));
        }

        [Theory]
        [InlineData(2019, 9, 1, "2019/2020 - S1")]
        [InlineData(2020, 2, 1, "2019/2020 - S2")]
        [InlineData(2020, 9, 1, "2020/2021 - S1")]
        [InlineData(2021, 2, 1, "2020/2021 - S2")]
        public void Period_to_string_shows_nice_formatting(int year, int month, int day, string expectedPeriod)
        {
            var period = Period.Create(new DateTime(year, month, day));

            var periodString = period.Value.ToString();

            periodString.Should().Be(expectedPeriod);
        }
    }
}