using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using StudentProgress.Core.UseCases;
using Xunit;

namespace StudentProgress.CoreTests.UseCases
{
    [Collection("db")]
    public class MilestonesUpdateLearningOutcomeTests : DatabaseTests
    {
        public MilestonesUpdateLearningOutcomeTests(DatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Changes_the_outcome_for_multiple_at_once_and_skips_already_existing_artefacts()
        {
            var group = Fixture.DataMother.CreateGroup(
                milestones: new[]
                {
                    ("1. a", "aaa"), // already existing
                    ("2. b", "aaaa"), // should change
                    ("3. c", "ccc"), // should remain the same
                    ("1. b", "aaa"), // should skip
                    ("3. c", "aaa") // should skip
                });
            Fixture.DataMother.CreateGroup(
                name: "existing group",
                milestones: new[]
                {
                    ("1. a", "aaaa"), // conflicting milestone that should not interfere
                });
            var milestone1 = group.Milestones.FirstOrDefault(m => m.LearningOutcome == "1. b" && m.Artefact == "aaa");
            var milestone2 = group.Milestones.FirstOrDefault(m => m.Artefact == "aaaa");
            var milestone3 = group.Milestones.FirstOrDefault(m => m.Artefact == "ccc");
            var milestone4 = group.Milestones.FirstOrDefault(m => m.LearningOutcome == "3. c" && m.Artefact == "aaa");
            await using var ucContext = Fixture.CreateDbContext();
            var useCase = new MilestonesUpdateLearningOutcome(ucContext);

            var result = await useCase.Handle(new MilestonesUpdateLearningOutcome.Command
            {
                GroupId = group.Id, LearningOutcome = "1. a",
                MilestoneIds = new List<int> {milestone1!.Id, milestone2!.Id, milestone4!.Id}
            });

            result.IsSuccess.Should().BeTrue();
            var actualGroup = Fixture.DataMother.GroupWithMilestones(group.Id);
            actualGroup.Milestones.FirstOrDefault(m => m.Id == milestone1.Id)!.LearningOutcome.Value.Should()
                .Be("1. b");
            actualGroup.Milestones.FirstOrDefault(m => m.Id == milestone2.Id)!.LearningOutcome.Value.Should()
                .Be("1. a");
            actualGroup.Milestones.FirstOrDefault(m => m.Id == milestone3!.Id)!.LearningOutcome.Value.Should()
                .Be("3. c");
            actualGroup.Milestones.FirstOrDefault(m => m.Id == milestone4.Id)!.LearningOutcome.Value.Should()
                .Be("3. c");
        }

        [Fact]
        public async Task Changes_the_outcome_for_all_at_once()
        {
            var group = Fixture.DataMother.CreateGroup(
                milestones: new[]
                {
                    ("1. b", "the excluded"),
                    ("1. a", "the first"),
                    ("1. a", "the second"),
                    ("1. a", "the third"),
                    ("1. a", "the fourth"),
                    ("1. a", "the fifth")
                });
            Fixture.DataMother.CreateGroup(
                name: "existing group",
                milestones: new[]
                {
                    ("1. a", "aaaa"), // conflicting milestone that should not interfere
                });

            await using var ucContext = Fixture.CreateDbContext();
            var useCase = new MilestonesUpdateLearningOutcome(ucContext);

            var result = await useCase.Handle(new MilestonesUpdateLearningOutcome.Command
            {
                GroupId = group.Id, LearningOutcome = "1. 1. a",
                MilestoneIds = group.Milestones.Where(m => m.LearningOutcome == "1. a").Select(m => m.Id).ToList()
            });

            result.IsSuccess.Should().BeTrue();
            var actualGroup = Fixture.DataMother.GroupWithMilestones(group.Id);
            actualGroup.Milestones.Count(m => m.LearningOutcome == "1. 1. a").Should().Be(5);
        }
    }
}