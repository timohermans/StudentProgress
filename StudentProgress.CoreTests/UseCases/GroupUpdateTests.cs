using System.Threading.Tasks;
using FluentAssertions;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;
using StudentProgress.CoreTests.Extensions;
using Xunit;

namespace StudentProgress.CoreTests.UseCases
{
    [Collection("db")]
    public class GroupUpdateTests : DatabaseTests
    {
        public GroupUpdateTests(DatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Can_update_group()
        {
            var group = Fixture.DataMother.CreateGroup();
            var request = new GroupUpdate.Request
            {
                Id = group.Id,
                Name = "S3 - Timo",
                Mnemonic = "Denk hier aan!"
            };
            var useCase = new GroupUpdate(new ProgressContext(Fixture.ContextOptions));

            var result = await useCase.HandleAsync(request);

            result.IsSuccess.Should().BeTrue();
            var groupUpdated = Fixture.DataMother.Query<StudentGroup>();
            groupUpdated
                .ShouldExist()
                .HasName("S3 - Timo")
                .HasMnemonic("Denk hier aan!");
        }

        [Fact]
        public async Task Cannot_update_non_existing_group()
        {
             var request = new GroupUpdate.Request
             {
                 Id = 1,
                 Name = "S3 - Timo",
             };
             var useCase = new GroupUpdate(new ProgressContext(Fixture.ContextOptions));
 
             var result = await useCase.HandleAsync(request);
 
             result.IsFailure.Should().BeTrue();
             result.Error.Should().Contain("exist");
        }
    }
}