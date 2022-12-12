using FluentAssertions;
using StudentProgress.Core.UseCases;
using StudentProgress.CoreTests.Extensions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace StudentProgress.CoreTests.UseCases
{
    [Collection("db")]
    public class MilestoneEditTests : DatabaseTests
    {
        public MilestoneEditTests(DatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Updates_a_milestone()
        {
            var group = Fixture.DataMother.CreateGroup(milestones: new[] { ("milestone 1", "Analyse document") });
            var milestone = group.Milestones.FirstOrDefault();
            await using var ucContext = Fixture.CreateDbContext();
            var uc = new MilestoneUpdate(ucContext);

            var result = await uc.Handle(new MilestoneUpdate.Command
            {
                Id = milestone!.Id,
                LearningOutcome = "2. Specifications and design",
                Artefact = "Design document",
            }, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            var dbGroup = Fixture.DataMother.GroupWithMilestones();
            dbGroup.ShouldExist();
            dbGroup!.Milestones.Should().ContainSingle();
            dbGroup
                .Milestones
                .First()
                .HasArtefact("Design document")
                .HasLearningOutcome("2. Specifications and design");
        }

        [Fact]
        public async Task Cannot_update_artefact_to_already_existing_on_learning_outcome()
        {
            var group = Fixture.DataMother.CreateGroup(milestones: new[] {
               ("2. Specifications and design", "Design document"),
               ("2. Specifications and design", "Analyse document"),
            });
            var milestone = group.Milestones.FirstOrDefault(m => m.Artefact == "Design document");
            await using var ucContext = Fixture.CreateDbContext();
            var uc = new MilestoneUpdate(ucContext);

            var result = await uc.Handle(new MilestoneUpdate.Command
            {
                Id = milestone!.Id,
                LearningOutcome = milestone!.LearningOutcome,
                Artefact = "Analyse document",
            }, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain("exists");
        }
    }
}
