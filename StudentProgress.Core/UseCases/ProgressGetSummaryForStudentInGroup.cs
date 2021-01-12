using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;

namespace StudentProgress.Core.UseCases
{
  public class ProgressGetSummaryForStudentInGroup
  {
    public record Query
    {
      public int GroupId { get; set; }
      public int StudentId { get; set; }
    }

    public record MilestoneResponse
    {
      public string Artefact { get; set; }
      public string LearningOutcome { get; set; }
      public Rating? Rating { get; set; }
      public string? Comment { get; set; }
      public int TimesWorkedOn { get; set; }

      public MilestoneResponse(string artefact, string learningOutcome, Rating? rating, string? comment, int timesWorkedOn)
      {
        Artefact = artefact;
        LearningOutcome = learningOutcome;
        Rating = rating;
        Comment = comment;
        TimesWorkedOn = timesWorkedOn;
      }
    }

    public record Response
    {
      public int GroupId { get; set; }
      public int StudentId { get; set; }
      public string GroupName { get; set; }
      public string StudentName { get; set; }
      public IEnumerable<MilestoneResponse> Milestones { get; set; }

      public Response(int groupId, int studentId, string groupName, string studentName, IEnumerable<MilestoneResponse> milestones)
      {
        GroupId = groupId;
        StudentId = studentId;
        GroupName = groupName;
        StudentName = studentName;
        Milestones = milestones;
      }
    }

    private readonly ProgressContext _context;

    public ProgressGetSummaryForStudentInGroup(ProgressContext context)
    {
      _context = context;
    }

    public async Task<Result<Response>> HandleAsync(Query query)
    {
      var group = Maybe<StudentGroup>.From(await _context.Groups.FindAsync(query.GroupId)).ToResult("Group does not exist");
      var student = Maybe<Student>.From(await _context.Students.FindAsync(query.StudentId)).ToResult("Student does not exist");
      var doGroupStudentExist = Result.Combine(group, student);

      if (doGroupStudentExist.IsFailure)
      {
        return Result.Failure<Response>(doGroupStudentExist.Error);
      }

      var milestones = await _context.Milestones.Where(m => m.StudentGroup.Id == query.GroupId).ToListAsync();
      var studentProgresses = await _context.MilestoneProgresses
                                .Include(mp => mp.Milestone)
                                .Where(mp => mp.ProgressUpdate.StudentId == query.StudentId && mp.Milestone.StudentGroup.Id == query.GroupId)
                                .OrderByDescending(mp => mp.ProgressUpdate.Date)
                                .ToListAsync();

      var milestonesSummary = milestones.Select(milestone =>
      {
        var milestoneProgresses = studentProgresses.Where(mp => mp.Milestone.Id == milestone.Id).ToList();
        var latestProgress = milestoneProgresses.FirstOrDefault();

        return new MilestoneResponse(
          artefact: milestone.Artefact,
          learningOutcome: milestone.LearningOutcome,
          rating: latestProgress?.Rating,
          comment: latestProgress?.Comment,
          timesWorkedOn: milestoneProgresses.Count
        );
      })
      .OrderBy(m => m.LearningOutcome)
      .ThenBy(m => m.Artefact)
      .ToList();

      return Result.Success(new Response(
        groupId: group.Value.Id,
        groupName: group.Value.Name,
        studentId: student.Value.Id,
        studentName: student.Value.Name,
        milestones: milestonesSummary
      ));
    }
  }
}