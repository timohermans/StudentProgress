using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;
using StudentProgress.CoreTests.Extensions;
using Xunit;

namespace StudentProgress.CoreTests.UseCases
{
    [Collection("db")]
    public class ProgressCreateTests : DatabaseTests
    {
        public ProgressCreateTests(DatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Creates_progress_for_a_student_of_a_group()
        {
            var group = Fixture.DataMother.CreateGroup(
                studentNames: new[] { "Timo" },
                milestones: new[] {
                      ("1. OO application", "DAL met SQLCommand"),
                      ("1. OO application", "SOLID principles"),
                      ("1. OO application", "Authentication")
                    }
                );
            var student = group.Students.FirstOrDefault();
            var request = new ProgressCreate.Command
            {
                GroupId = group.Id,
                StudentId = student?.Id ?? 0,
                Date = new DateTime(2020, 12, 18),
                Feedback = "Come on!",
                Feedforward = "Work on this!",
                Feedup = "Good job!",
                Feeling = Feeling.Neutral,
                Milestones = new List<ProgressCreate.MilestoneProgressCommand>
                {
                    new ProgressCreate.MilestoneProgressCommand { Id = group.Milestones.FirstOrDefault(m => m.Artefact == "DAL met SQLCommand")!.Id, Rating = null, Comment = null },
                    new ProgressCreate.MilestoneProgressCommand { Id = group.Milestones.FirstOrDefault(m => m.Artefact == "SOLID principles")!.Id, Rating = Rating.Orienting, Comment = "Still not what it should be" }
                }
            };
            using var ucContext = Fixture.CreateDbContext();
            var useCase = new ProgressCreate(ucContext);

            var result = await useCase.HandleAsync(request);

            result.IsSuccess.Should().BeTrue();
            var progress = Fixture.DataMother.QueryProgressUpdateWithMilestonesProgress();
            progress
                .ShouldExist()
                .HasDate(new DateTime(2020, 12, 18))
                .HasFeedback("Come on!")
                .HasFeedforward("Work on this!")
                .HasFeedup("Good job!")
                .HasFeeling(Feeling.Neutral)
                .HasMilestonesProgressCount(1)
                .HasMilestoneProgressRatingAt(0, Rating.Orienting)
                .HasMilestoneProgressCommentAt(0, "Still not what it should be");
        }

        [Fact]
        public async Task Cannot_create_for_non_existing_group()
        {
            var group = Fixture.DataMother.CreateGroup();
            var request = new ProgressCreate.Command
            {
                GroupId = group.Id,
                StudentId = 55,
                Date = new DateTime(2020, 12, 18),
                Feedback = "Come on!",
                Feedforward = "Work on this!",
                Feedup = "Good job!",
                Feeling = Feeling.Neutral
            };
            var useCase = new ProgressCreate(Fixture.CreateDbContext());

            var result = await useCase.HandleAsync(request);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain("exist");
        }
    }
}