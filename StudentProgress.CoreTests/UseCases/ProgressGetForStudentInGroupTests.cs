using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using StudentProgress.Core.UseCases;
using Xunit;

namespace StudentProgress.CoreTests.UseCases
{
    [Collection("db")]
    public class ProgressGetForStudentInGroupTests : DatabaseTests
    {
        public ProgressGetForStudentInGroupTests(DatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Gets_the_progress_for_the_student_by_date()
        {
            var group = Fixture.DataMother.CreateGroup(studentNames: "Timo");
            var student = group.Students.FirstOrDefault();
            Fixture.DataMother.CreateProgressUpdate(group, student,
                date: new DateTime(2020, 2, 2));
            Fixture.DataMother.CreateProgressUpdate(group, student,
                date: new DateTime(2020, 1, 1));
            Fixture.DataMother.CreateProgressUpdate(group, student,
                date: new DateTime(2020, 3, 3));
            var useCase = new ProgressGetForStudentInGroup(Fixture.CreateDbContext());

            var result = await useCase.HandleAsync(new ProgressGetForStudentInGroup.Request(group.Id, student?.Id));

            result.StudentId.Should().Be(student?.Id);
            result.Name.Should().Be("Timo");
            result.GroupId.Should().Be(group.Id);
            result.GroupName.Should().Be(group.Name);
            result.ProgressUpdates.Count().Should().Be(3);
            result.ProgressUpdates.ElementAt(0)!.Date.Should().Be(new DateTime(2020, 3, 3));
            result.ProgressUpdates.ElementAt(1)!.Date.Should().Be(new DateTime(2020, 2, 2));
            result.ProgressUpdates.ElementAt(2)!.Date.Should().Be(new DateTime(2020, 1, 1));
        }
    }
}