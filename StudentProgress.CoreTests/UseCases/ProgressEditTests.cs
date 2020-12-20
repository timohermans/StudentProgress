using System;
using System.Threading.Tasks;
using FluentAssertions;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;
using StudentProgress.CoreTests.Extensions;
using Xunit;

namespace StudentProgress.CoreTests.UseCases
{
    [Collection("db")]
    public class ProgressEditTests : DatabaseTests
    {
        public ProgressEditTests(DatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Edits_existing_progress()
        {
            var progress = Fixture.DataMother.CreateProgressUpdate();
            var useCase = new ProgressEdit(Fixture.CreateDbContext());
            var request = new ProgressEdit.Request
            {
                Date = new DateTime(1991, 1, 16),
                Feedback = "feedback 1",
                Feedforward = "feedforward 1",
                Feedup = "feedup 1",
                Feeling = Feeling.Bad,
                Id = progress.Id
            };

            var result = await useCase.HandleAsync(request);

            result.IsSuccess.Should().BeTrue();
            var update = Fixture.DataMother.Query<ProgressUpdate>()
                .ShouldExist()
                .HasDate(new DateTime(1991, 1, 16))
                .HasFeedback("feedback 1")
                .HasFeedforward("feedforward 1")
                .HasFeedup("feedup 1")
                .HasFeeling(Feeling.Bad);
        }
    }
}