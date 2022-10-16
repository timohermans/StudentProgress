using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using StudentProgress.Core.Entities;
using System.Threading;

namespace StudentProgress.Core.UseCases
{
    public class ProgressGetSummaryForStudentInGroup : IUseCaseBase<ProgressGetSummaryForStudentInGroup.Query, Result<ProgressGetSummaryForStudentInGroup.Response>>
    {
        public record Query : IUseCaseRequest<Result<Response>>
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

            public MilestoneResponse(string artefact, string learningOutcome, Rating? rating, string? comment,
                int timesWorkedOn)
            {
                Artefact = artefact;
                LearningOutcome = learningOutcome;
                Rating = rating;
                Comment = comment;
                TimesWorkedOn = timesWorkedOn;
            }
        }

        public record ProgressUpdateResponse(int Id, DateTime Date, Feeling Feeling, int StudentId, int GroupId);

        public record OtherStudentResponse(int Id, string Name);

        public record Response
        {
            public int GroupId { get; set; }
            public int StudentId { get; set; }
            public string GroupName { get; set; }
            public string StudentName { get; set; }
            public string? StudentAvatarPath { get; set; }
            public string? StudentNote { get; set; }
            public Period Period { get; set; }
            public string? LastFeedback { get; }
            public DateTime? LastFeedbackDate { get; }
            public IEnumerable<MilestoneResponse> Milestones { get; }
            public IEnumerable<ProgressUpdateResponse> ProgressUpdates { get; }
            public IEnumerable<OtherStudentResponse> OtherStudents { get; }

            public Response(int groupId, int studentId, string groupName, string studentName, string? studentAvatarPath,
                string? studentNote,
                Period period,
                IEnumerable<MilestoneResponse> milestones, IEnumerable<ProgressUpdateResponse> progressUpdates,
                IEnumerable<OtherStudentResponse> otherStudents, string? lastFeedback, DateTime? lastFeedbackDate)
            {
                GroupId = groupId;
                StudentId = studentId;
                GroupName = groupName;
                StudentName = studentName;
                StudentAvatarPath = studentAvatarPath;
                StudentNote = studentNote;
                Period = period;
                Milestones = milestones;
                ProgressUpdates = progressUpdates;
                OtherStudents = otherStudents;
                LastFeedback = lastFeedback;
                LastFeedbackDate = lastFeedbackDate;
            }
        }

        private readonly ProgressContext _context;

        public ProgressGetSummaryForStudentInGroup(ProgressContext context)
        {
            _context = context;
        }

        public async Task<Result<Response>> Handle(Query query, CancellationToken token)
        {
            var group = Maybe<StudentGroup>.From(await _context.Groups.FindAsync(query.GroupId))
                .ToResult("Group does not exist");
            var student = Maybe<Student>.From(await _context.Students.FindAsync(query.StudentId))
                .ToResult("Student does not exist");
            var doGroupStudentExist = Result.Combine(group, student);

            if (doGroupStudentExist.IsFailure)
            {
                return Result.Failure<Response>(doGroupStudentExist.Error);
            }

            var milestones = await _context.Milestones.Where(m => m.StudentGroup.Id == query.GroupId).ToListAsync(token);
            var studentMilestoneProgresses = await _context.MilestoneProgresses
                .Include(mp => mp.Milestone)
                .Where(mp =>
                    mp.ProgressUpdate.StudentId == query.StudentId && mp.Milestone.StudentGroup.Id == query.GroupId)
                .OrderByDescending(mp => mp.ProgressUpdate.Date)
                .ToListAsync(token);
            var progressUpdates = await _context.ProgressUpdates
                .Where(u => u.GroupId == query.GroupId && u.StudentId == query.StudentId)
                .ToListAsync(token);

            var lastProgressUpdate = progressUpdates
                .Where(p => p.Feedback != null)
                .OrderByDescending(p => p.Date)
                .FirstOrDefault();

            var milestonesSummary = milestones.Select(milestone =>
                {
                    var milestoneProgresses =
                        studentMilestoneProgresses.Where(mp => mp.Milestone.Id == milestone.Id).ToList();
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

            var otherStudents = await _context.Students
                .Where(s => s.StudentGroups.Any(g => g.Id == query.GroupId))
                .OrderBy(s => s.Name)
                .Select(s => new OtherStudentResponse(s.Id, s.Name))
                .ToListAsync(token);

            return Result.Success(new Response(
                groupId: group.Value.Id,
                groupName: group.Value.Name,
                studentId: student.Value.Id,
                studentName: student.Value.Name,
                studentAvatarPath: student.Value.AvatarPath,
                studentNote: student.Value.Note,
                milestones: milestonesSummary,
                progressUpdates: progressUpdates.Select(u =>
                    new ProgressUpdateResponse(u.Id, u.Date, u.ProgressFeeling, u.StudentId, u.GroupId)).ToList(),
                period: group.Value.Period,
                otherStudents: otherStudents,
                lastFeedback: lastProgressUpdate?.Feedback,
                lastFeedbackDate: lastProgressUpdate?.Date
            ));
        }
    }
}