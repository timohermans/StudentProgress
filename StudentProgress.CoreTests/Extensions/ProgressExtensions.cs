using System;
using System.Linq;
using FluentAssertions;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;

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

    public static ProgressUpdate HasFeeling(this ProgressUpdate update, Feeling feeling)
    {
      update.ProgressFeeling.Should().Be(feeling);
      return update;
    }

    public static ProgressUpdate HasMilestonesProgressCount(this ProgressUpdate update, int count)
    {
      update.MilestonesProgress.Should().HaveCount(count);
      return update;
    }

    public static ProgressUpdate HasMilestoneProgressIdAt(this ProgressUpdate update, int index, int id)
    {
      update.MilestonesProgress.ElementAt(index).Id.Should().Be(id);
      return update;
    }

    public static ProgressUpdate HasMilestoneProgressRatingAt(this ProgressUpdate update, int index, Rating rating)
    {
      update.MilestonesProgress.ElementAt(index).Rating.Should().Be(rating);
      return update;
    }

    public static ProgressUpdate HasMilestoneProgressCommentAt(this ProgressUpdate update, int index, string comment)
    {
      update.MilestonesProgress.ElementAt(index).Comment.Should().Be(comment);
      return update;
    }

    public static ProgressUpdate IsReviewed(this ProgressUpdate update)
    {
      update.IsReviewed.Should().BeTrue();
      return update;
    }
    
    public static ProgressUpdate IsNotReviewed(this ProgressUpdate update)
    {
      update.IsReviewed.Should().BeFalse();
      return update;
    }

    public static ProgressCreateOrUpdate.MilestoneProgressCommand GetMilestoneProgress(this ProgressCreateOrUpdate.Command command, int milestoneId)
    {
      return command.Milestones.First(m => m.MilestoneId == milestoneId);
    }
  }
}