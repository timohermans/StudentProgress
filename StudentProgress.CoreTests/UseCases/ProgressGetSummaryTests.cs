using System;
using System.Linq;
using System.Net.Cache;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;
using Xunit;

namespace StudentProgress.CoreTests.UseCases
{
    [Collection("db")]
    public class ProgressGetSummaryTests : DatabaseTests
    {
        public ProgressGetSummaryTests(DatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Gets_latest_progress_updates_per_milestone()
        {
            // arrange
            var group = Fixture.DataMother.CreateGroup(
                name: "S3 2020-2021",
                milestones: new[]
                {
                    ("first", "a. 1"),
                    ("first", "b. 2"),
                    ("first", "c. 3")
                },
                students: new[] { new TestStudent("Timo", null, "timo.png"), new TestStudent("Max"), new TestStudent("Jordy") }
            );
            var nonInterestingGroup = Fixture.DataMother.CreateGroup(name: "not interesting group", students: new[] { new TestStudent("Timothy") });
            var timo = group.Students[0];
            var max = group.Students.FirstOrDefault(s => s.Name == "Max");
            var jordy = group.Students.FirstOrDefault(s => s.Name == "Jordy");
            var timothy = nonInterestingGroup.Students[0];
            var milestoneA = group.Milestones.First(m => m.Artefact == "a. 1");
            var milestoneB = group.Milestones.First(m => m.Artefact == "b. 2");
            var milestoneC = group.Milestones.First(m => m.Artefact == "c. 3");
            var updateA = Fixture.DataMother.CreateProgressUpdate(group, timo,
                date: new DateTime(2021, 1, 11),
                feedback: "feedback from A",
                milestoneProgresses: new[]
                {
                    new MilestoneProgress(Rating.Orienting, milestoneA, "orienting"),
                    new MilestoneProgress(Rating.Undefined, milestoneB, "undefined"),
                }
            );
            var updateB = Fixture.DataMother.CreateProgressUpdate(group, timo,
                date: new DateTime(2021, 2, 22),
                feedback: null,
                milestoneProgresses: new[]
                {
                    new MilestoneProgress(Rating.Advanced, milestoneA, null)
                }
            );
            using var ucContext = Fixture.CreateDbContext();
            var useCase = new ProgressGetSummaryForStudentInGroup(ucContext);

            // act
            var result = await useCase.Handle(new ProgressGetSummaryForStudentInGroup.Query
            { GroupId = group.Id, StudentId = timo.Id }, CancellationToken.None);

            // assert
            result.IsSuccess.Should().BeTrue();
            var summary = result.Value;
            summary.GroupId.Should().Be(group.Id);
            summary.GroupName.Should().Be(group.Name);
            summary.StudentId.Should().Be(timo.Id);
            summary.StudentName.Should().Be(timo.Name);
            summary.StudentAvatarPath.Should().Be(timo.AvatarPath);
            summary.Period.Should().Be(group.Period);
            summary.Milestones.Should().HaveCount(3);
            summary.Milestones.Should().Contain(new ProgressGetSummaryForStudentInGroup.MilestoneResponse
            {
                IdLastProgressUpdate = updateB.Id,
                Artefact = "a. 1",
                LearningOutcome = "first",
                Rating = Rating.Advanced,
                Comment = null,
                TimesWorkedOn = 2
            });

            summary.Milestones.Should().Contain(new ProgressGetSummaryForStudentInGroup.MilestoneResponse
            {
                IdLastProgressUpdate = updateA.Id,
                Artefact = "b. 2",
                LearningOutcome = "first",
                Rating = Rating.Undefined,
                Comment = "undefined",
                TimesWorkedOn = 1
            });
            summary.Milestones.Should()
                .Contain(new ProgressGetSummaryForStudentInGroup.MilestoneResponse
                {
                    IdLastProgressUpdate = null,
                    Artefact = "c. 3",
                    LearningOutcome = "first",
                    Rating = null,
                    Comment = null,
                    TimesWorkedOn = 0
                });
            summary.ProgressUpdates.Should().HaveCount(2);
            summary.ProgressUpdates.Should().Contain(new ProgressGetSummaryForStudentInGroup.ProgressUpdateResponse(
                updateA.Id,
                    new DateTime(2021, 1, 11), updateA.ProgressFeeling, updateA.StudentId, updateA.GroupId));
            summary.ProgressUpdates.Should().Contain(new ProgressGetSummaryForStudentInGroup.ProgressUpdateResponse(
                updateB.Id,
                new DateTime(2021, 2, 22), updateB.ProgressFeeling, updateB.StudentId, updateB.GroupId));
            summary.OtherStudents.Should().Contain(
                new ProgressGetSummaryForStudentInGroup.OtherStudentResponse(jordy!.Id, jordy!.Name));
            summary.OtherStudents.Should().Contain(
                new ProgressGetSummaryForStudentInGroup.OtherStudentResponse(max!.Id, max!.Name));
            summary.OtherStudents.Should().Contain(
                new ProgressGetSummaryForStudentInGroup.OtherStudentResponse(timo!.Id, timo!.Name));
            summary.OtherStudents.Should().NotContain(
                new ProgressGetSummaryForStudentInGroup.OtherStudentResponse(timothy!.Id, timothy!.Name));
            summary.LastFeedback.Should().Be("feedback from A");
            summary.LastFeedbackDate.Should().Be(new DateTime(2021, 1, 11));
        }
    }
}