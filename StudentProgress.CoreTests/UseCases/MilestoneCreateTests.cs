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
    public class MilestoneCreateTests : DatabaseTests
    {
        public MilestoneCreateTests(DatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Creates_a_milestone()
        {
            var group = Fixture.DataMother.CreateGroup();
            Fixture.DataMother.CreateGroup(
                name: "Conflicting Group that should be no problem",
                milestones: new[] {("2. Specifications and design", "Design document")});
            await using var ucContext = Fixture.CreateDbContext();
            var uc = new MilestoneCreate(ucContext);

            var result = await uc.HandleAsync(new MilestoneCreate.Command
            {
                LearningOutcome = "2. Specifications and design",
                Artefact = "Design document",
                GroupId = group.Id
            });

            result.IsSuccess.Should().BeTrue();
            Fixture.DataMother.GroupWithMilestones()
                .Milestones
                .FirstOrDefault()
                .HasArtefact("Design document")
                .HasLearningOutcome("2. Specifications and design");
        }

        [Fact]
        public async Task Cannot_create_artefact_per_learning_outcome_twice()
        {
            var group = Fixture.DataMother.CreateGroup(milestones: new[]
            {
                ("2. Specifications and design", "Design document")
            });
            await using var ucContext = Fixture.CreateDbContext();
            var uc = new MilestoneCreate(ucContext);

            var result = await uc.HandleAsync(new MilestoneCreate.Command
            {
                LearningOutcome = "2. Specifications and design",
                Artefact = "Design document",
                GroupId = group.Id
            });

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain("exists");
        }
    }
}