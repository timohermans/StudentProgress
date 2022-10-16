using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;
using Xunit;

namespace StudentProgress.CoreTests.UseCases
{
    [Collection("db")]
    public class MilestoneDeleteTest : DatabaseTests
    {
        public MilestoneDeleteTest(DatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Deletes_a_milestone_with_progress_from_a_group()
        {
            var group = Fixture.DataMother.CreateGroup(
                students: new[] {new TestStudent("Timo")},
                milestones: new[] {("1. a", "artefact 1")}
            );
            var student = group.Students.FirstOrDefault();
            var milestone = group.Milestones.FirstOrDefault();
            Fixture.DataMother.CreateProgressUpdate(group, student, milestoneProgresses: new[]
            {
                new MilestoneProgress(Rating.Orienting, milestone, "goodbye")
            });
            await using var ucDb = Fixture.CreateDbContext();
            var useCase = new MilestoneDelete(ucDb);

            await useCase.Handle(new MilestoneDelete.Command {Id = milestone.Id});

            var groupAfterDeletion = Fixture.DataMother.GroupWithMilestones();
            groupAfterDeletion.ShouldExist();
            groupAfterDeletion.Milestones.Should().HaveCount(0);
        }
    }
}