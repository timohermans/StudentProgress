using FluentAssertions;
using StudentProgress.Core.UseCases;
using StudentProgress.CoreTests.Extensions;
using System.Linq;
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

            var result = await uc.HandleAsync(new MilestoneUpdate.Command
            {
                Id = milestone!.Id,
                LearningOutcome = "2. Specifications and design",
                Artefact = "Design document",
            });

            result.IsSuccess.Should().BeTrue();
            var dbGroup = Fixture.DataMother.GroupWithMilestones();
            dbGroup.Milestones.Should().ContainSingle();
            dbGroup
                .Milestones
                .FirstOrDefault()
                .HasArtefact("Design document")
                .HasLearningOutcome("2. Specifications and design");
        }
    }
}
