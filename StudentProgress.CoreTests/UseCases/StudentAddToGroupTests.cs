using System.Threading.Tasks;
using FluentAssertions;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;
using StudentProgress.CoreTests.Extensions;
using Xunit;

namespace StudentProgress.CoreTests.UseCases
{
    [Collection("db")]
    public class StudentAddToGroupTests : DatabaseTests
    {
        public StudentAddToGroupTests(DatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Adds_student_to_group()
        {
            var group = Fixture.DataMother.CreateGroup();
            var request = new StudentAddToGroup.Request
            {
                Name = "Timo Hermans",
                GroupId = group.Id
            };
            var useCase = new StudentAddToGroup(Fixture.CreateDbContext());

            var result = await useCase.HandleAsync(request);

            result.IsSuccess.Should().BeTrue();
            Fixture.DataMother.GroupWithStudents()
                .ShouldExist()
                .HasStudent("Timo Hermans");
        }

        [Fact]
        public async Task Fails_to_add_student_that_is_already_added()
        {
            var group = Fixture.DataMother.CreateGroup(studentNames: new[] {"Timo Hermans"});
            var request = new StudentAddToGroup.Request
            {
                Name = "Timo Hermans",
                GroupId = group.Id
            };
            var useCase = new StudentAddToGroup(Fixture.CreateDbContext());

            var result = await useCase.HandleAsync(request);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain("already");
        }
    }
}