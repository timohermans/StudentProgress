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
    public class StudentUpdateTests : DatabaseTests
    {
        public StudentUpdateTests(DatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Updates_the_user_with_a_note()
        {
            var group = Fixture.DataMother.CreateGroup(students: new[] {new TestStudent("TimO")});
            var student = group.Students.FirstOrDefault();
            await using var dbContext = Fixture.CreateDbContext();
            var useCase = new StudentUpdate(dbContext);

            var result = await useCase.Handle(new StudentUpdate.Command
                {Id = student!.Id, Name = "Timo", Note = "Everything is going just fine"}, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            var actualStudent = Fixture.DataMother.Query<Student>();
            actualStudent.Name.Should().Be("Timo");
            actualStudent.Note.Should().Be("Everything is going just fine");
        }

        [Fact]
        public async Task Cannot_update_a_user_to_a_user_that_already_exists()
        {
            var group = Fixture.DataMother.CreateGroup(students: new[] {new TestStudent("Tiimo"), new TestStudent("Timo")});
            var studentWrongName = group.Students.First(g => g.Name == "Tiimo");
            await using var dbContext = Fixture.CreateDbContext();
            var useCase = new StudentUpdate(dbContext);

            var result = await useCase.Handle(new StudentUpdate.Command {Id = studentWrongName.Id, Name = "Timo"}, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain("already exists");
        }
    }
}