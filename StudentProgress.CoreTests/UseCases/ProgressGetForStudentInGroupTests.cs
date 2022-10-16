using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;
using Xunit;

namespace StudentProgress.CoreTests.UseCases
{
    [Collection("db")]
    public class ProgressGetForStudentInGroupTests : DatabaseTests
    {
        public ProgressGetForStudentInGroupTests(DatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Gets_the_progress_for_the_student_by_date()
        {
            // arrange
            var group = Fixture.DataMother.CreateGroup(
                students: new TestStudent("Timo"),
                milestones: new[] { ("1. Application", "SOLID patterns"), ("2. Professional", "Acting"), ("3. Algorithms", "Circustrain") });
            var student = group.Students.FirstOrDefault();
            var solidMilestone = group.Milestones.FirstOrDefault(m => m.Artefact == "SOLID patterns");
            var actingMilestone = group.Milestones.FirstOrDefault(m => m.Artefact == "Acting");
            var algoMilestone = group.Milestones.FirstOrDefault(m => m.Artefact == "Circustrain");

            Fixture.DataMother.CreateProgressUpdate(
                group, student,
                date: new DateTime(2020, 2, 2),
                milestoneProgresses: new[] {
                    new MilestoneProgress(Rating.Advanced, solidMilestone, "Awesome dev"),
                    new MilestoneProgress(Rating.Beginning, actingMilestone, "More feedback"),
                });

            Fixture.DataMother.CreateProgressUpdate(
                group, student,
                date: new DateTime(2020, 1, 1),
                milestoneProgresses: new[] {
                    new MilestoneProgress(Rating.Proficient, algoMilestone, "sufficient")
                });

            Fixture.DataMother.CreateProgressUpdate(
                group, student,
                feedback: "bad",
                date: new DateTime(2020, 3, 3));

            using var ucContext = Fixture.CreateDbContext();
            var useCase = new ProgressGetForStudentInGroup(ucContext);

            var result = await useCase.Handle(new ProgressGetForStudentInGroup.Request(group.Id, student?.Id), CancellationToken.None);

            result.StudentId.Should().Be(student?.Id);
            result.Name.Should().Be("Timo");
            result.GroupId.Should().Be(group.Id);
            result.GroupName.Should().Be(group.Name);
            result.ProgressUpdates.Count().Should().Be(3);
            result.ProgressUpdates.ElementAt(0)!.Date.Should().Be(new DateTime(2020, 3, 3));
            result.ProgressUpdates.ElementAt(0)!.Feedback.Should().Be("bad");
            result.ProgressUpdates.ElementAt(0)!.MilestoneProgresses.Should().HaveCount(0);
            result.ProgressUpdates.ElementAt(1)!.Date.Should().Be(new DateTime(2020, 2, 2));
            result.ProgressUpdates.ElementAt(1)!.MilestoneProgresses.Should().HaveCount(2);
            result.ProgressUpdates.ElementAt(2)!.Date.Should().Be(new DateTime(2020, 1, 1));
            result.ProgressUpdates.ElementAt(2)!.MilestoneProgresses.Should().HaveCount(1);
            result.ProgressUpdates.ElementAt(2)!.MilestoneProgresses.FirstOrDefault()!.Rating.Should().Be(Rating.Proficient);
            result.ProgressUpdates.ElementAt(2)!.MilestoneProgresses.FirstOrDefault()!.LearningOutcome.Should().Be("3. Algorithms");
            result.ProgressUpdates.ElementAt(2)!.MilestoneProgresses.FirstOrDefault()!.Artefact.Should().Be("Circustrain");
            result.ProgressUpdates.ElementAt(2)!.MilestoneProgresses.FirstOrDefault()!.Comment.Should().Be("sufficient");
        }
    }
}