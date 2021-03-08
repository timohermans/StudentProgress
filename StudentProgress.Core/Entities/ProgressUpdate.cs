﻿using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;

namespace StudentProgress.Core.Entities
{
  public class ProgressUpdate : AuditableEntity<int>
  {
    public Student Student { get; private set; }
    public StudentGroup Group { get; private set; }
    public string? Feedback { get; private set; }
    public Feeling ProgressFeeling { get; private set; }
    public int GroupId { get; private set; }
    public int StudentId { get; private set; }
    public DateTime Date { get; private set; }
    private readonly List<MilestoneProgress> _milestonesProgress = new();
    public IReadOnlyList<MilestoneProgress> MilestonesProgress => _milestonesProgress;
    private readonly List<ProgressTag> _tags = new();
    public IReadOnlyList<ProgressTag> Tags => _tags;

#nullable disable
    private ProgressUpdate() { }
#nullable enable

    public ProgressUpdate(Student student, StudentGroup group, string? feedback, Feeling progressFeeling, DateTime date)
    {
      Student = student ?? throw new NullReferenceException(nameof(student));
      Group = group ?? throw new NullReferenceException(nameof(group));
      Feedback = feedback;
      ProgressFeeling = progressFeeling;
      GroupId = group.Id;
      Date = date;
    }

    public Result Update(Feeling feeling, DateTime date, string? feedback)
    {
      ProgressFeeling = feeling;
      Feedback = feedback;
      Date = date;

      return Result.Success();
    }

    public Result AddMilestones(IEnumerable<MilestoneProgress> milestones)
    {
      _milestonesProgress.AddRange(milestones);
      return Result.Success();
    }

    public UpdateMilestoneProgressesResult UpdateMilestoneProgressesWith(List<MilestoneProgress> commandMilestoneProgresses)
    {
      var toRemove = _milestonesProgress.Where(pmp => !commandMilestoneProgresses.Any(cmp => cmp.Milestone == pmp.Milestone)).ToList();
      var newProgresses = commandMilestoneProgresses.Where(cpu => !MilestonesProgress.Any(mp => mp.Milestone == cpu.Milestone));
      var toUpdateProgresses = commandMilestoneProgresses.Where(cpu => MilestonesProgress.Any(mp => mp.Milestone == cpu.Milestone));

      toRemove.ForEach(mp => _milestonesProgress.Remove(mp));
      foreach (var milestoneProgress in toUpdateProgresses)
      {
        var progress = _milestonesProgress.FirstOrDefault(mp => mp.Milestone == milestoneProgress.Milestone);
        progress!.Update(milestoneProgress.Comment, milestoneProgress.Rating);
      }
      AddMilestones(newProgresses);
      return new UpdateMilestoneProgressesResult { RemovedMilestoneProgresses = toRemove };
    }
  }

  public record UpdateMilestoneProgressesResult
  {
    public List<MilestoneProgress> RemovedMilestoneProgresses { get; init; } = new();
  }
}
