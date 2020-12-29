using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;
using Xunit;

namespace StudentProgress.CoreTests.UseCases
{
    [Collection("db")]
    public class GroupCreateTests : DatabaseTests
    {
        public GroupCreateTests(DatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Can_create_a_group()
        {
            var request = new GroupCreate.Request
            {
                Name = "S3 - Leon",
                Mnemonic = null
            };
            var useCase = new GroupCreate(new ProgressContext(Fixture.ContextOptions));

            var result = await useCase.HandleAsync(request);

            Assert.True(result.IsSuccess);
            using var assertDb = new ProgressContext(Fixture.ContextOptions);
            var group = assertDb.Groups.FirstOrDefault();
            group.Should().NotBe(null);
            group.Id.Should().Be(1);
            group.Name.Value.Should().Be("S3 - Leon");
            group.Mnemonic.Should().Be(null);
        }

        [Fact]
        public async Task Cannot_create_a_duplicate_group()
        {
            Fixture.DataMother.CreateGroup("S3-Leon");
            var request = new GroupCreate.Request
            {
                Name = "S3-Leon",
                Mnemonic = "Dit is een test"
            };
            var useCase = new GroupCreate(new ProgressContext(Fixture.ContextOptions));

            var result = await useCase.HandleAsync(request);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain("already exists");
        }

    }
}