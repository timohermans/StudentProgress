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
            await using var ucContext = Fixture.CreateDbContext();
            var uc = new MilestoneCreate(ucContext);

            var result = await uc.HandleAsync(new MilestoneCreate.Request
            {
                Name = "Design document",
                GroupId = group.Id
            });

            result.IsSuccess.Should().BeTrue();
            Fixture.DataMother.GroupWithMilestones()
                .Milestones
                .FirstOrDefault()
                .HasName("Design document");
        }
    }
}