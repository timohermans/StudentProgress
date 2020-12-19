using System;
using FluentAssertions;
using StudentProgress.Core.Entities;

namespace StudentProgress.CoreTests.Extensions
{
    public static class ProgressExtensions
    {
        public static ProgressUpdate HasDate(this ProgressUpdate update, DateTime date)
        {
            update.Date.Should().Be(date);
            return update;
        }

        public static ProgressUpdate HasFeedback(this ProgressUpdate update, string feedback)
        {
            update.Feedback.Should().Be(feedback);
            return update;
        }

        public static ProgressUpdate HasFeedforward(this ProgressUpdate update, string feedforward)
        {
            update.Feedforward.Should().Be(feedforward);
            return update;
        }

        public static ProgressUpdate HasFeedup(this ProgressUpdate update, string feedup)
        {
            update.Feedup.Should().Be(feedup);
            return update;
        }

        public static ProgressUpdate HasFeeling(this ProgressUpdate update, Feeling feeling)
        {
            update.ProgressFeeling.Should().Be(feeling);
            return update;
        }
    }
}