using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using FluentAssertions;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;
using StudentProgress.CoreTests.Extensions;
using Xunit;

namespace StudentProgress.CoreTests.UseCases
{
    [Collection("db")]
    public class ProgressCreateEditTests : DatabaseTests
    {
        public ProgressCreateEditTests(DatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Creates_progress_for_a_student_of_a_group()
        {
            var group = Fixture.DataMother.CreateGroup(
                students: new[] { new TestStudent("Timo") },
                milestones: new[]
                {
                    ("1. OO application", "DAL met SQLCommand"),
                    ("1. OO application", "SOLID principles"),
                    ("1. OO application", "Authentication")
                }
            );
            var student = group.Students.FirstOrDefault();
            var request = new ProgressCreateOrUpdate.Command
            {
                GroupId = group.Id,
                StudentId = student?.Id ?? 0,
                Date = new DateTime(2020, 12, 18),
                Feedback = "Come on!",
                Feeling = Feeling.Neutral,
                IsReviewed = false,
                Milestones = new List<ProgressCreateOrUpdate.MilestoneProgressCommand>
                {
                    new ProgressCreateOrUpdate.MilestoneProgressCommand
                    {
                        MilestoneId = group.Milestones.FirstOrDefault(m => m.Artefact == "DAL met SQLCommand")!.Id,
                        Rating = null, Comment = null
                    },
                    new ProgressCreateOrUpdate.MilestoneProgressCommand
                    {
                        MilestoneId = group.Milestones.FirstOrDefault(m => m.Artefact == "SOLID principles")!.Id,
                        Rating = Rating.Orienting, Comment = "Still not what it should be"
                    }
                }
            };
            using var ucContext = Fixture.CreateDbContext();
            var useCase = new ProgressCreateOrUpdate(ucContext);

            var result = await useCase.HandleAsync(request);

            result.IsSuccess.Should().BeTrue();
            var progress = Fixture.DataMother.QueryProgressUpdateWithMilestonesProgress();
            progress
                .ShouldExist()
                .HasDate(new DateTime(2020, 12, 18))
                .HasFeedback("Come on!")
                .HasFeeling(Feeling.Neutral)
                .IsNotReviewed()
                .HasMilestonesProgressCount(1)
                .HasMilestoneProgressRatingAt(0, Rating.Orienting)
                .HasMilestoneProgressCommentAt(0, "Still not what it should be");
        }

        [Fact]
        public async Task Cannot_create_for_non_existing_group()
        {
            var group = Fixture.DataMother.CreateGroup();
            var request = new ProgressCreateOrUpdate.Command
            {
                GroupId = group.Id,
                StudentId = 55,
                IsReviewed = true,
                Date = new DateTime(2020, 12, 18),
                Feedback = "Come on!",
                Feeling = Feeling.Neutral
            };
            var useCase = new ProgressCreateOrUpdate(Fixture.CreateDbContext());

            var result = await useCase.HandleAsync(request);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain("exist");
        }

        [Fact]
        public async Task Updates_the_progress_update_successfully()
        {
            // arrange
            var group = Fixture.DataMother.CreateGroup(
                students: new[] { new TestStudent("Timo") },
                milestones: new[]
                {
                    ("1. Feedback van stakeholders", "Compleetheid documentatie"),
                    ("1. Feedback van stakeholders", "Onderbouwing beslissingen"),
                    ("2. Samenwerking en communicatie", "Samenwerking/communicatie"),
                    ("3. Algorithms", "Circustrein")
                }
            );
            var milestoneCompleteness = group.Milestones.FirstOrDefault(m => m.Artefact == "Compleetheid documentatie");
            var milestoneArgumentation =
                group.Milestones.FirstOrDefault(m => m.Artefact == "Onderbouwing beslissingen");
            var milestoneCooperation = group.Milestones.FirstOrDefault(m => m.Artefact == "Samenwerking/communicatie");
            var milestoneAlgorithm = group.Milestones.FirstOrDefault(m => m.Artefact == "Circustrein");
            var student = group.Students.FirstOrDefault();
            var progress = Fixture.DataMother.CreateProgressUpdate(
                group, student,
                feedback: "Or is it?",
                feeling: Feeling.Bad,
                date: new DateTime(2021, 1, 7),
                isReviewed: false,
                milestoneProgresses: new List<MilestoneProgress>
                {
                    new MilestoneProgress(Rating.Undefined, milestoneCompleteness,
                        "Hij begrijpt het echt nog helemaal niet"),
                    new MilestoneProgress(Rating.Orienting, milestoneArgumentation, null),
                    new MilestoneProgress(Rating.Beginning, milestoneCooperation, "Communicatie is perfect"),
                });
            using var getContext = Fixture.CreateDbContext();
            var command = (await new ProgressGetForCreateOrUpdate(getContext).HandleAsync(
                new ProgressGetForCreateOrUpdate.Query
                    { GroupId = group.Id, StudentId = student.Id, Id = progress.Id })).Value.Command;

            command.Date = new DateTime(2021, 2, 11);
            command.Feedback = "back";
            command.Feeling = Feeling.Good;
            command.IsReviewed = true;
            command.Milestones = command.Milestones.Where(m => m.MilestoneId != milestoneCooperation.Id).ToList();
            command.GetMilestoneProgress(milestoneCompleteness.Id).Comment = "completeness";
            command.GetMilestoneProgress(milestoneCompleteness.Id).Rating = Rating.Advanced;
            command.GetMilestoneProgress(milestoneArgumentation.Id).Comment = "argumentation";
            command.GetMilestoneProgress(milestoneArgumentation.Id).Rating = Rating.Undefined;

            using var ucContext = Fixture.CreateDbContext();
            var useCase = new ProgressCreateOrUpdate(ucContext);

            // act
            var result = await useCase.HandleAsync(command);

            // assert
            result.IsSuccess.Should().BeTrue();

            var actualProgressUpdate = Fixture.DataMother.QueryProgressUpdateWithMilestonesProgress();
            actualProgressUpdate
                .HasDate(new DateTime(2021, 2, 11))
                .HasFeedback("back")
                .HasFeeling(Feeling.Good)
                .IsReviewed()
                .HasMilestonesProgressCount(2)
                .HasMilestoneProgressIdAt(0,
                    progress.MilestonesProgress.FirstOrDefault(mp => mp.Milestone == milestoneCompleteness)!.Id)
                .HasMilestoneProgressRatingAt(0, Rating.Advanced)
                .HasMilestoneProgressCommentAt(0, "completeness")
                .HasMilestoneProgressIdAt(1,
                    progress.MilestonesProgress.FirstOrDefault(mp => mp.Milestone == milestoneArgumentation)!.Id)
                .HasMilestoneProgressRatingAt(1, Rating.Undefined)
                .HasMilestoneProgressCommentAt(1, "argumentation");
        }
    }
}