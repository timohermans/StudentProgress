using System;
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
            var group = Fixture.DataMother.CreateGroup(studentNames: new[] {"Timo"});
            var student = group.Students.FirstOrDefault();
            var request = new ProgressCreate.Request
            {
                GroupId = group.Id,
                StudentId = student?.Id ?? 0,
                Date = new DateTime(2020, 12, 18),
                Feedback = "Come on!",
                Feedforward = "Work on this!",
                Feedup = "Good job!",
                Feeling = Feeling.Neutral
            };
            var useCase = new ProgressCreate(Fixture.CreateDbContext());

            var result = await useCase.HandleAsync(request);

            result.IsSuccess.Should().BeTrue();
            var progress = Fixture.DataMother.Query<ProgressUpdate>();
            progress
                .ShouldExist()
                .HasDate(new DateTime(2020, 12, 18))
                .HasFeedback("Come on!")
                .HasFeedforward("Work on this!")
                .HasFeedup("Good job!")
                .HasFeeling(Feeling.Neutral);
        }

        [Fact]
        public async Task Cannot_create_for_non_existing_group()
        {
            var group = Fixture.DataMother.CreateGroup();
            var request = new ProgressCreate.Request
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