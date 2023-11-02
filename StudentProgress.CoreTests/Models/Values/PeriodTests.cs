using StudentProgress.CoreTests.Lib.Infrastructure;
using StudentProgress.Web.Models.Values;

namespace StudentProgress.CoreTests.Models.Values
{
    public class PeriodTests
    {
        [Theory]
        [InlineData(2023, 1, 1, 2023, 1, 1, "2223vj")]
        [InlineData(2023, 7, 10, 2023, 7, 10, "2324nj")]
        public void Period_tests(int createYear, int createMonth,
            int createDay,
            int expectedYear, int expectedMonth, int expectedDay, string expectedFormat)
        {
            var period = new Period { StartDate = new DateTime(createYear, createMonth, createDay) };

            period.StartDate.Should().Be(new DateTime(expectedYear, expectedMonth, expectedDay));
            period.ToString().Should().Be(expectedFormat);
        }

        [Theory]
        [InlineData(2023, 9, 1, 2023, 10, 29, false)]
        [InlineData(2023, 9, 1, 2024, 1, 29, false)]
        [InlineData(2023, 9, 1, 2024, 2, 1, true)]
        [InlineData(2023, 2, 1, 2023, 2, 1, false)]
        [InlineData(2023, 2, 1, 2023, 6, 29, false)]
        [InlineData(2023, 2, 1, 2023, 7, 1, true)]
        [InlineData(2025, 2, 1, 2023, 2, 1, false)]
        [InlineData(2025, 9, 1, 2023, 9, 29, false)]
        public void Marks_period_as_over(int createYear, int createMonth, int createDay,
            int todayYear, int todayMonth, int todayDay, bool expectedIsOver)
        {
            var today = new DateTime(todayYear, todayMonth, todayDay);
            var dateProvider = new DummyDateProvider(today);
            var period = new Period { StartDate = new DateTime(createYear, createMonth, createDay) };

            period.IsOver(dateProvider).Should().Be(expectedIsOver);
        }
        
        [Theory]
        [InlineData(2023, 9, 1, 2023, 8, 29, true)]
        [InlineData(2023, 9, 1, 2024, 1, 29, false)]
        public void Marks_period_as_not_started_yet(int createYear, int createMonth, int createDay,
            int todayYear, int todayMonth, int todayDay, bool expectedIsNotStarted)
        {
            var today = new DateTime(todayYear, todayMonth, todayDay);
            var dateProvider = new DummyDateProvider(today);
            var period = new Period { StartDate = new DateTime(createYear, createMonth, createDay) };

            period.HasNotStartedYet(dateProvider).Should().Be(expectedIsNotStarted);
        }

        // [Theory]
        // [InlineData(2019, 9, 1, "2019/2020 - S1")]
        // [InlineData(2020, 2, 1, "2019/2020 - S2")]
        // [InlineData(2020, 9, 1, "2020/2021 - S1")]
        // [InlineData(2021, 2, 1, "2020/2021 - S2")]
        // public void Period_to_string_shows_nice_formatting(int year, int month, int day, string expectedPeriod)
        // {
        //     var period = Period.Create(new DateTime(year, month, day));
        //
        //     var periodString = period.Value.ToString();
        //
        //     periodString.Should().Be(expectedPeriod);
        // }

        // [Theory]
        // [InlineData(2021, 1, 16, 2020, 8, 31)]
        // [InlineData(2021, 2, 8, 2021, 2, 8)]
        // [InlineData(2021, 8, 30, 2021, 8, 30)]
        // public void Creates_the_active_period_of_a_given_date(int year, int month, int day, int expectedYear,
        //     int expectedMonth, int expectedDay)
        // {
        //     var period = Period.CreateCurrentlyActivePeriodBy(new DateTime(year, month, day));
        //
        //     period.Value.StartDate.Should().Be(new DateTime(expectedYear, expectedMonth, expectedDay));
        // }

        // [Theory]
        // [InlineData(2021, 2, 1, 2021, 2, 9, 1)]
        // [InlineData(2021, 2, 1, 2021, 2, 10, 2)]
        // [InlineData(2021, 2, 1, 2023, 2, 1, 202)]
        // public void Gives_the_amount_of_days_passed_in_the_semester(int year, int month, int day, int sinceYear, int sinceMonth, int sinceDay, int actualDaysPassed)
        // {
        //     var period = Period.Create(new DateTime(year, month, day)).Value;
        //
        //     var daysPassed = period.TimePassedInsideSemesterSince(new DateTime(sinceYear, sinceMonth, sinceDay));
        //
        //     daysPassed.Should().Be(actualDaysPassed.Days());
        // }
    }
}