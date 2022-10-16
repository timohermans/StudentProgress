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
    public class MilestonesCopyFromGroupTests : DatabaseTests
    {
        public MilestonesCopyFromGroupTests(DatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Copies_milestones_from_one_group_to_the_other()
        {
            var groupToCopyFrom = Fixture.DataMother.CreateGroup(milestones: new[]
                {("1. a", "artefact 1"), ("2. b", "artefact 2")});

            var groupToCopyTo = Fixture.DataMother.CreateGroup("Group new",
                milestones: new[] {("1. a", "artefact 1 old"), ("3. c", "artefact 3")});
            await using var db = Fixture.CreateDbContext();
            var useCase = new MilestonesCopyFromGroup(db);

            var result = await useCase.Handle(new MilestonesCopyFromGroup.Command
                {FromGroupId = groupToCopyFrom.Id, ToGroupId = groupToCopyTo.Id}, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            var group = Fixture.DataMother.GroupWithMilestones(groupToCopyTo.Id);
            group.Milestones.Should().HaveCount(4);
            group.Milestones.FirstOrDefault(m => m.LearningOutcome == "1. a" && m.Artefact == "artefact 1")!.Should()
                .NotBeNull();
            group.Milestones.FirstOrDefault(m => m.LearningOutcome == "2. b")!.Artefact.Should()
                .Be((Name) "artefact 2");
            group.Milestones.FirstOrDefault(m => m.LearningOutcome == "3. c")!.Artefact.Should()
                .Be((Name) "artefact 3");
        }
    }
}